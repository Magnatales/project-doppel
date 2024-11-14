using Godot;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public partial class Test : Node
{
    private const int Iterations = 10000;  // Run each test multiple times to magnify performance differences

    public override void _Ready()
    {
        TestAsyncPerformance();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    private async void TestAsyncPerformance()
    {
        // Measure performance with try-catch
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            await TestAsyncWithTryCatch();
        }
        stopwatch.Stop();
        GD.Print($"With try-catch (x{Iterations}): {stopwatch.ElapsedMilliseconds} ms");

        // Measure performance without try-catch
        stopwatch.Restart();
        for (int i = 0; i < Iterations; i++)
        {
            await TestAsyncWithoutTryCatch();
        }
        stopwatch.Stop();
        GD.Print($"Without try-catch (x{Iterations}): {stopwatch.ElapsedMilliseconds} ms");
    }

    private async Task TestAsyncWithTryCatch()
    {
        try
        {
            await Task.Yield(); // Reduced delay to focus on try-catch overhead
        }
        catch (Exception e)
        {
            GD.PrintErr(e);  // Error logging if needed
        }
    }

    private async Task TestAsyncWithoutTryCatch()
    {
        await Task.Yield(); // Same short delay for consistency
    }
}