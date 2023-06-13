package dev.mantikafasi.MLock;

import static android.content.Context.MODE_PRIVATE;

import android.app.PendingIntent;
import android.appwidget.AppWidgetManager;
import android.appwidget.AppWidgetProvider;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Looper;
import android.widget.RemoteViews;
import android.widget.Toast;

import java.util.Objects;

/**
 * Implementation of App Widget functionality.
 */
public class MLockWidget extends AppWidgetProvider {
    private static final String OnLock = "onLock";
    private static final String OnUnlock = "OnUnlock";
    SharedPreferences sharedPreferences;
    RemoteViews views;
    android.os.Handler mainHandler = new android.os.Handler(Looper.getMainLooper());
    Context ctx;

    void updateAppWidget(Context context, AppWidgetManager appWidgetManager,
                         int appWidgetId) {

        views = new RemoteViews(context.getPackageName(), R.layout.m_lock_widget);

        views.setOnClickPendingIntent(R.id.buttonLock, getPendingSelfIntent(context, OnLock));
        views.setOnClickPendingIntent(R.id.buttonUnlock, getPendingSelfIntent(context, OnUnlock));
        // Instruct the widget manager to update the widget
        appWidgetManager.updateAppWidget(appWidgetId, views);
    }

    protected PendingIntent getPendingSelfIntent(Context context, String action) {
        Intent intent = new Intent(context, getClass());
        intent.setAction(action);
        return PendingIntent.getBroadcast(context, 0, intent, 0);
    }

    @Override
    public void onReceive(Context context, Intent intent) {
        if (!intent.getAction().equals(OnLock) && !intent.getAction().equals(OnUnlock)) {
            super.onReceive(context, intent);
            return;
        }

        ctx = context;

        sharedPreferences = context.getSharedPreferences("MLock", MODE_PRIVATE);
        String ip = sharedPreferences.getString("ipAddress", "");
        String pass = sharedPreferences.getString("password", "");
        if (ip.isEmpty() || pass.isEmpty()) {
            toast("Please set IP and Password from app");
            return;
        }

        new Thread(() -> {
            try {
                API.makeApiRequest(
                        Objects.equals(intent.getAction(), OnLock)
                                ? API.RequestType.LOCK
                                : API.RequestType.UNLOCK,
                        ip,
                        pass
                );
                toast("Successfully" + (Objects.equals(intent.getAction(), OnLock) ? "Locked" : "Unlocked" + "computer"));
            } catch (Exception e) {
                mainHandler.post(() -> toast(e.getMessage()));
            }
        }).start();

        super.onReceive(context, intent);
    }

    public void toast(String text) {
        mainHandler.post(() ->
                Toast.makeText(ctx, text, text.length() < 15 ? Toast.LENGTH_SHORT : Toast.LENGTH_LONG).show());
    }

    @Override
    public void onUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds) {
        // There may be multiple widgets active, so update all of them
        for (int appWidgetId : appWidgetIds) {
            updateAppWidget(context, appWidgetManager, appWidgetId);
        }
    }
}