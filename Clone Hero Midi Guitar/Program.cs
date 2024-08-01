using System;
using NAudio.Midi;
using WindowsInput;
using WindowsInput.Native;

namespace MidiInputExample
{
    class Program
    {
        
        private static InputSimulator inputSimulator = new InputSimulator();

        
        private static VirtualKeyCode[] midiToKeyMap = new VirtualKeyCode[]
        {
            VirtualKeyCode.VK_A, // MIDI note 60 -> A
            VirtualKeyCode.VK_S, // MIDI note 62 -> S
            VirtualKeyCode.VK_D, // MIDI note 77 -> D
            VirtualKeyCode.VK_F, // MIDI note 79 -> F
            VirtualKeyCode.VK_G  // MIDI note 81 -> G
            // This is configured to my keyboard piano
        };

        static void Main(string[] args)
        {
            
            int deviceCount = MidiIn.NumberOfDevices;
            Console.WriteLine("Available MIDI input devices:");
            for (int i = 0; i < deviceCount; i++)
            {
                Console.WriteLine($"{i}: {MidiIn.DeviceInfo(i).ProductName}");
            }

            
            if (deviceCount > 0)
            {
                // Open the first available MIDI input device
                using (MidiIn midiIn = new MidiIn(0))
                {
                    
                    midiIn.MessageReceived += MidiIn_MessageReceived;

                    // Starts receiving MIDI input
                    midiIn.Start();

                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("No MIDI input devices found.");
            }
        }

        private static void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                
                int midiNoteNumber = noteOnEvent.NoteNumber;
                Console.WriteLine($"Note On: {midiNoteNumber}");

                
                SimulateKeyPress(midiNoteNumber, true);
            }
            else if (e.MidiEvent is NoteEvent noteEvent && noteEvent.CommandCode == MidiCommandCode.NoteOff)
            {
                
                int midiNoteNumber = noteEvent.NoteNumber;
                Console.WriteLine($"Note Off: {midiNoteNumber}");

                
                SimulateKeyPress(midiNoteNumber, false);
            }
        }

        private static void SimulateKeyPress(int midiNoteNumber, bool keyDown)
        {
           
            int[] validNotes = { 60, 62, 77, 79, 81 };
            int index = Array.IndexOf(validNotes, midiNoteNumber);

            if (index >= 0 && index < midiToKeyMap.Length)
            {
                VirtualKeyCode keyCode = midiToKeyMap[index];
                if (keyDown)
                {
                    inputSimulator.Keyboard.KeyDown(keyCode);
                    inputSimulator.Keyboard.KeyDown(VirtualKeyCode.UP); 
                }
                else
                {
                    inputSimulator.Keyboard.KeyUp(keyCode);
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.UP); 
                }
            }
        }
    }
}
