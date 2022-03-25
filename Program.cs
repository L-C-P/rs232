using Figgle;
using System.IO.Ports;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 4)
        {

            Console.WriteLine(FiggleFonts.Standard.Render("Simple RS232"));
            Console.WriteLine("rs232send command port options text");

            return;
        }

        string command = args[0];

        switch (command.ToUpperInvariant())
        {
            case "LIST":
                List();
                break;
            case "SEND":
                Send(args[1], args[2], args[3]);
                break;
            // case "SWITCH":
            //     Send(args[1], "baud=57600,data=8,parity=None,newline=true", $"EZS OUT{args[2]} VS IN{args[3]}");
            //     break;
            default:
                Console.WriteLine($"Unknown command {command}");
                break;
        }

    }

    private static void List()
    {
        Console.WriteLine("Ports:");
        string[] portNames = SerialPort.GetPortNames();
        Console.WriteLine(String.Join(Environment.NewLine, portNames));
    }

    private static void Send(string portName, string optionsParam, string text)
    {
        SerialPort serialPort = new SerialPort();

        if (portName.StartsWith('*'))
        {
            portName = portName[1..];
            Console.WriteLine($"Search for {portName}");

            Regex regex = new Regex($".*{portName}.*");
            portName = "";
            string[] portNames = SerialPort.GetPortNames();

            foreach (string port in portNames)
            {
                if (regex.IsMatch(port))
                {
                    portName = port;
                    Console.WriteLine($"Using {portName}");

                    break;
                }
            }
            if (portName == "")
            {
                Console.WriteLine("No matching port found");
                return;
            }
        }

        serialPort.PortName = portName;
        string[] options = optionsParam.Split(',');
        bool newLine = false;

        foreach (string option in options)
        {
            string[] vs = option.Split('=');
            string parameter = vs.First();
            string value = vs.Last();

            switch (parameter.ToUpperInvariant())
            {
                case "BAUD":
                    serialPort.BaudRate = int.Parse(value);
                    break;
                case "DATA":
                    serialPort.DataBits = int.Parse(value);
                    break;
                case "STOP":
                    serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), value);
                    break;
                case "PARITY":
                    serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), value);
                    break;
                case "HANDSHAKE":
                    serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), value);
                    break;
                case "NEWLINE":
                    newLine = bool.Parse(value);
                    break;
            }
        }

        serialPort.ReadTimeout = 500;
        serialPort.WriteTimeout = 500;
        serialPort.Open();

        if (serialPort.IsOpen)
        {
            Console.WriteLine($"Send {text} to {portName}");
            
            if (newLine)
                serialPort.WriteLine(text);
            else
                serialPort.Write(text);

            serialPort.Close();
        }
        else
            Console.WriteLine($"Port {portName} not opened");
    }
}