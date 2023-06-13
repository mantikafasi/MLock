package dev.mantikafasi.MLock;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.SocketTimeoutException;
import java.net.URL;

public class API {
    public static void makeApiRequest(RequestType requestType, String API_URL, String password) throws Exception {
        String suffix = "";

        switch (requestType) {
            case LOCK:
                suffix = "/lock";
                break;
            case UNLOCK:
                suffix = "/unlock";
                break;
        }

        suffix += "?password=" + password;
        String finalSuffix = suffix;
        HttpURLConnection connection = null;

        try {
            URL url = new URL("http://" + API_URL + finalSuffix);
            connection = (HttpURLConnection) url.openConnection();
            connection.setConnectTimeout(1500); // We dont want it to wait forever
            connection.setRequestMethod("GET");

            int responseCode = connection.getResponseCode();
            switch (responseCode) {
                case HttpURLConnection.HTTP_OK:
                    break;
                case HttpURLConnection.HTTP_UNAUTHORIZED:
                    throw new Exception("Wrong password");
                default:
                    throw new Exception("An error occured while processing your request");
            }

        } catch (SocketTimeoutException e) {
            e.printStackTrace();
            throw new Exception("Couldnt connect to the server, are you sure MLock is running and accessible?");
        } catch (IOException e) {
            e.printStackTrace();
            throw new Exception("An error occured while processing your request");
        } finally {
            if (connection != null) {
                connection.disconnect();
            }
        }
    }

    enum RequestType {
        LOCK,
        UNLOCK
    }
}
