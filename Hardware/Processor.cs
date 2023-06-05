namespace Hardware;

// the BLIP processor is based on the instruction set and behaviour of the MOS 6502.
// it is also simplified because we don't need every single instruction.

using Register = Byte;

public class Processor
{
    public Processor() { }

    private static Register A = 0;
    private static Register B = 0;
    private static Register C = 0;

    private static byte  StackPointer       = 0; // 0x1FF - 0x0100
    private static byte  Flag               = 0;
    private static short InstructionPointer = 0; // also known as the program counter

    // -------------------
    // Implied addressing
    // -------------------

    // 0x40
    public void RTI()
    {
        Flag = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte highIP = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte lowIP  = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        InstructionPointer = (short)((short)(highIP << 8) + (short)(lowIP));
    }

    // 0x48
    public void PHA()
    {
        StackPointer--;
        Memory.SetByteAtAddress((short)(0x01FF-StackPointer), A);
    }

    // 0x08
    public void PHP()
    {
        StackPointer--;
        Memory.SetByteAtAddress((short)(0x01FF - StackPointer), Flag);
    }

    // 0x68
    public void PLA()
    {
        A = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
    }

    // 0x28
    public void PLP()
    {
        Flag = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
    }

    // 0xAA
    public void TAB()
    {
        B = A;
    }

    // 0xA8
    public void TAC()
    {
        C = A;
    }

    // 0xBA
    public void TSB()
    {
        B = StackPointer;
    }

    // 0x8A
    public void TBA()
    {
        A = B;
    }

    //0x9A
    public void TBS()
    {
        StackPointer = B;
    }

    // 0x98
    public void TCA()
    {
        A = C;
    }

    // 0x38
    public void SEC()
    {
        Flag |= (1 << 0);
    }

    // 0xF8
    public void SED()
    {
        Flag |= (1 << 3);
    }

    // 0x78
    public void SEI()
    {
        Flag |= (1 << 2);
    }

    // 0xEA
    public void NOP()
    {
        // do nothing
    }

    // 0xCA
    public void DEB()
    {
        B--;
    }

    // 0x88
    public void DEC()
    {
        C--;
    }

    // 0xE8
    public void INB()
    {
        B++;
    }

    // 0xC8
    public void INC()
    {
        C++;
    }

    // 0x00
    public void BRK()
    {
        byte lowIP = (byte)((InstructionPointer+2 << 8) >> 8);
        Memory.SetByteAtAddress((short)(0x1FFF - StackPointer), lowIP);
        StackPointer--;

        byte highIP = (byte)(InstructionPointer+2 >> 8);
        Memory.SetByteAtAddress((short)(0x1FFF - StackPointer), highIP);
        StackPointer--;

        unchecked {
            lowIP  = Memory.GetByteAtAddress((short)(0xFFFE));
            highIP = Memory.GetByteAtAddress((short)(0xFFFF));
        }
        InstructionPointer = (short)((short)(highIP << 8) + (short)(lowIP));
    }

    // 0x18
    public void CLC()
    {
        Flag &= 1;
    }

    // 0xD8
    public void CLD()
    {
        Flag &= (1 << 3);
    }

    // 0x58
    public void CLI()
    {
        Flag &= (1 << 2);
    }

    // 0xB8
    public void CLV()
    {
        Flag &= (1 << 6);
    }

    // -------------------
    // A register addressing
    // -------------------

    // 0x0A
    public void ASL()
    {
        if ((A | 0x7F) == 1) Flag++; // Sets the Carry flag, which is the lowest bit so this works.
        Flag |= (byte)((A << 2) >> 7);
        A <<= 1;
    }
}