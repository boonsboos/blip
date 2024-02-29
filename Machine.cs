namespace blip;

using blip.Hardware;
using Video;

class Machine
{
    public static void Main() {

        new Screen();
        Processor p = new Processor();
        Console.WriteLine("joe");
        p.Cycle(0b00000_00001_000000000001_000_0010011);
        while( true) {
        }
    }
}
