namespace Video;

using Raylib_CsLo;

// BLIP ships with a 4:3 aspect ratio screen.
// its resolution is 480 by 360.

public class Screen
{
    public Task ?ScreenThread {get;}

    public Screen()
    {
        this.ScreenThread = Task.Factory.StartNew( () => {
            Raylib.InitWindow(960, 720, "Hello, Raylib-CsLo");
            Raylib.SetTargetFPS(24);
            Raylib.SetTraceLogLevel(4); // LOG_WARNING
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Raylib.GRAY);
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        });
    }

}