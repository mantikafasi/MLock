package dev.mantikafasi.MLock;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import dev.mantikafasi.MLock.databinding.ActivityMainBinding;

public class MainActivity extends AppCompatActivity {
    ActivityMainBinding binding;
    EditText ipAddressEditText;
    EditText passwordEditText;

    SharedPreferences sharedPreferences;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        binding = ActivityMainBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        sharedPreferences = getSharedPreferences("MLock", MODE_PRIVATE);

        ipAddressEditText = findViewById(R.id.editTextIPAddress);
        passwordEditText = findViewById(R.id.editTextPassword);

        ipAddressEditText.setText(sharedPreferences.getString("ipAddress", ""));
        passwordEditText.setText(sharedPreferences.getString("password", ""));
    }

    public void onUnlockComputerButtonClicked(View v) {
        if (!validateInput()) {
            return;
        }

        new Thread(() -> {
            try {
                API.makeApiRequest(API.RequestType.UNLOCK, ipAddressEditText.getText().toString(), passwordEditText.getText().toString());
                toast("Successfully unlocked");
            } catch (Exception e) {
                toast(e.getMessage());
            }
        }).start();
    }

    public void onLockComputerButtonClicked(View v) {
        if (!validateInput()) {
            return;
        }

        new Thread(() -> {
            try {
                API.makeApiRequest(API.RequestType.LOCK, ipAddressEditText.getText().toString(), passwordEditText.getText().toString());
                toast("Successfully locked");
            } catch (Exception e) {
                toast(e.getMessage());
            }
        }).start();
    }

    public void toast(String text) {
        runOnUiThread(() ->
                Toast.makeText(MainActivity.this, text, text.length() < 15 ? Toast.LENGTH_SHORT : Toast.LENGTH_LONG).show());
    }

    public boolean validateInput() {
        String ipAdress = ipAddressEditText.getText().toString();
        String password = passwordEditText.getText().toString();

        if (ipAdress.isEmpty() || password.isEmpty()) {
            Toast.makeText(MainActivity.this, "Please fill the form above", Toast.LENGTH_SHORT).show();
            return false;
        }
        sharedPreferences.edit().putString("ipAddress", ipAdress).putString("password", password).apply();
        return true;
    }
}
