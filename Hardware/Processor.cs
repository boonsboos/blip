namespace blip.Hardware;

using Raylib_CsLo;

// based on RISC-V 1.0 ISA and MOS 6502
public class Processor
{
    public static byte[] memory = new byte[32 * 1024 * 1024]; // 32 mib of ram
    // no virtual address space, purely physical.

    private uint[]   Registers = new uint[32];
    private float[]  FloatRegs = new float[32];

    // stack pointer is register x30.

    private uint   InstructionPointer = 0; // also known as the program counter
    // instruction pointer should never be memory length or greater

    private static readonly Dictionary<uint, InstructionType> instTypes = new Dictionary<uint, InstructionType>() {
        /* Control transfer */
        {0b1100111, InstructionType.J}, // J imm25
        {0b1101111, InstructionType.J}, // JAL imm25
        {0b1101011, InstructionType.I}, // JALR.C/R/J and RDNPC
        /* Memory */
        {0b0000011, InstructionType.I}, // L[BHW](U) rd, rs1, imm12
        {0b0100011, InstructionType.B}, // S[BHW] imm12hi, rs1, rs2, imm12lo
        /* Integer compute */
        {0b0010011, InstructionType.I}, // all I-type compute instructions
    };

    public void Cycle(uint instruction) {
        // TODO: fetch instruction instead of passing it in

        decodeAndExecuteInstruction(instruction);

        InstructionPointer += 4;
    }

    // all RISC-V 1.0 instructions are 32 bits wide
    private void decodeAndExecuteInstruction(uint instruction) {
        uint opcode = instruction & ((1 << 7) - 1); // get lower 7 bits

        switch(instTypes.GetValueOrDefault(opcode)) {
            case InstructionType.J:

                break;
            case InstructionType.LUI:
                break;
            case InstructionType.I:
                decodeAndExecuteITypeInstruction(instruction, opcode);
                break;
            case InstructionType.B:
                break;
            case InstructionType.R:
                break;
            case InstructionType.R4:
                break;
        }
    }

    private void decodeAndExecuteITypeInstruction(uint instruction, uint opcode) {

        uint option = 0b111 & (instruction >> 7);
        int imm12 = (int) (0b111111111111 & (instruction >> 10));
        if ((imm12 & (1 << 11)) == 1) {
            imm12 &= ~(1<<11);
            imm12 *= -1;
        }
        // imm12 is sign extended to 16 bits

        uint rs1    = 0b11111 & (instruction >> 22);
        uint rd     = 0b11111 & (instruction >> 27);

        switch (opcode) {
            case 0b0000011: // LOAD
                LD(option, imm12, rs1, rd);
                break;
            case 0b0010011: // all I Type Compute Instructions
                IntegerComputeForI(option, imm12, rs1, rd);
                break;
            case 0b1101011:
                JALR(option, imm12, rs1, rd);
                break;
            default:
                throw new CPUTrap($"Opcode {opcode} is not an I-type instruction.");
        }

        Raylib.DrawText(
            opcode.ToString() + "\n" +
            option.ToString() + "\n" +
            imm12.ToString() + "\n" +
            rs1.ToString() + "\n"+
            rd.ToString(),
            1, 1, 14, Raylib.RAYWHITE
        );
    }

    private void LD(uint option, int imm12, uint rs1, uint rd) {
        switch (option) {
            case 0b000: // LB sign extend
                Registers[rd] = memory[Registers[rs1]+imm12];
                break;
            case 0b001: // LH sign extend
                Registers[rd] = ByteUtil.FromBytes(
                    memory[Registers[rs1]+imm12],
                    memory[Registers[rs1]+imm12+1]
                );
                break;
            case 0b010: // LW sign extend
                Registers[rd] = ByteUtil.FromBytes(
                    memory[
                        (int)(Registers[rs1]+imm12)..(int)(Registers[rs1]+imm12+3)
                    ]
                );
                break;
            case 0b100: // LB zero extend
                Registers[rd] = memory[Registers[rs1]+imm12];
                if (Registers[rd] > sbyte.MaxValue) { // flip the MSB
                    Registers[rd] ^= 0x80000000;
                }
                break;
            case 0b101: // LH zero extend
                Registers[rd] = ByteUtil.FromBytes(
                    memory[Registers[rs1]+imm12],
                    memory[Registers[rs1]+imm12+1]
                );
                if (Registers[rd] > short.MaxValue) { // flip the MSB
                    Registers[rd] ^= 0x80000000;
                }
                break;
            case 0b110: // LW zero extend
                Registers[rd] = ByteUtil.FromBytes(
                    memory[
                        (int)(Registers[rs1]+imm12)..(int)(Registers[rs1]+imm12+3)
                    ]
                );
                if (Registers[rd] > int.MaxValue) { // flip the MSB
                    Registers[rd] ^= 0x80000000;
                }
                break;
        }
    }

    private void IntegerComputeForI(uint option, int imm12, uint rs1, uint rd) {
        switch (option) {
            case 0b000: // ADDI
                Registers[rd] = (uint)((int) Registers[rs1] + imm12);
                // convert to int, then add those together
                // next write the signed (sign-extended) value to the register
                break;
            case 0b001: // SLLI
                if (imm12 >= 32) throw new CPUTrap("SLLI imm12 > 32");
                Registers[rd] = Registers[rs1] << imm12;
                break;
            case 0b010: // SLTI
                if ((int) Registers[rs1] < imm12) {
                    Registers[rd] = 1;
                } else {
                    Registers[rd] = 0;
                }
                break;
            case 0b011: // SLTIU
                if (Registers[rs1] < (uint) imm12) {
                    Registers[rd] = 1;
                } else {
                    Registers[rd] = 0;
                }
                break;
            case 0b100: // XORI
                Registers[rd] = Registers[rs1] ^ (uint) imm12;
                break;
            case 0b101: // SRLI
                if (imm12 >= 32) throw new CPUTrap("SLRI imm12 > 32");
                Registers[rd] = Registers[rs1] >> imm12;
                break;
            case 0b110: // ORI
                Registers[rd] = Registers[rs1] | (uint) imm12;
                break;
            case 0b111: // ANDI
                Registers[rd] = Registers[rs1] & (uint) imm12;
                break;
        }
    }

    private void JALR(uint option, int imm12, uint rs1, uint rd) {
        switch (option) {
            // these mean the exact same things
            case 0b000: // JALR.C for subroutines
            case 0b001: // JALR.R for returning
            case 0b010: // JALR.J for indirect jumping
                InstructionPointer = (uint)((long) Registers[rs1] + imm12);
                Registers[rd] = InstructionPointer + 4;
                break;
            case 0b100: // RDNPC
                Registers[rs1] = InstructionPointer + 4;
                break;
            default: 
                throw new CPUTrap($"JALR does not support option {option}");
        }
    }
}

enum InstructionType
{
    J,
    LUI,
    I,
    B,
    R,
    R4
}

/// <summary>
/// Converts little endian memory to values 
/// </summary>
public class ByteUtil
{
    public static ushort FromBytes(byte a, byte b) {
        return (ushort)((b << 8) + a);
    }

    public static byte[] ToBytes(ushort a) {
        return [(byte)(a & 0xFF), (byte)(a >> 8)];
    }

    public static float FloatFromBytes(byte[] a) {
        return (
            (a[3] << 24) + 
            (a[2] << 16) + 
            (a[1] << 8) + 
            a[0]
        );
    }

    public static ushort FromBytes(byte[] a) {
        return (ushort)( 
            (a[1] << 8) + 
            a[0]
        );
    }

    public static byte[] ToBytes(uint a) {
        return [
            (byte)(a & 0x000000FF),
            (byte)(a & 0x0000FF00 >> 8),
            (byte)(a & 0x00FF0000 >> 16),
            (byte)(a >> 24)
        ];
    }
}

class CPUTrap : Exception
{
    public CPUTrap(string message): base(message) { }
}