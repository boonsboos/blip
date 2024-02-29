namespace blip.Video;

using Raylib_CsLo;

// BLIP ships with a 4:3 aspect ratio screen.
// its resolution is 480 by 360.

public class Screen
{
    public static Task ?ScreenThread {get;set;} // DANGER: do not change the screen thread.

    public Screen()
    {
        ScreenThread = Task.Factory.StartNew( () => {
            Raylib.InitWindow(760, 480, "blip");
            Raylib.SetTargetFPS(100);
            Raylib.SetTraceLogLevel(4); // LOG_WARNING

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Raylib.BLACK);
                Raylib.DrawRectangle(120, 0, 640, 480, Raylib.GRAY);
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        });
    }

}