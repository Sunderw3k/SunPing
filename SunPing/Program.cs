using System.Net.NetworkInformation;
using System.Text;
using System.Net;

namespace SunPing
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Help
            if (args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine("\nUsage: sping <Address (string)> <BufferSize (int)> <Timeout (ms)> <TTL (int)>\nIf an argument isn't specified, the default value is used (1.1.1.1, 32, 10000, 128)");
                return;
            }
            Console.Clear();

            // Default values
            string address = "1.1.1.1";
            int buffersize = 32;
            int timeout = 10000;
            int ttl = 128;

            // Replacing default values with user values
            if (args.Length >= 1)
            {
                address = args[0];
            }
            if (args.Length >= 2 && int.TryParse(args[1], out int argBuffersize))
            {
                buffersize = argBuffersize;
            }
            if (args.Length >= 3 && int.TryParse(args[2], out int argTimeout))
            {
                timeout = argTimeout;
            }
            if (args.Length == 4 && int.TryParse(args[3], out int argTtl))
            {
                ttl = argTtl;
            }

            // Making the app look better
            float _timeout = float.Parse(timeout.ToString());
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = $"[SunPing] - Pinging {address} with {buffersize} bytes - Timeout: {_timeout / 1000}s - TTL: {ttl}";

            // Handling CTRL+C
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => { Console.ForegroundColor = ConsoleColor.White; Console.Clear(); e.Cancel = false; };

            // Creating the bytes
            byte[] buffer = Encoding.ASCII.GetBytes(Multiply("a", buffersize));

            // Sening the packets and printing the response using Ping()
            while (true)
            {
                Console.WriteLine("================================");
                Console.WriteLine($"Pinging Target IP \"{address}\"");
                Console.WriteLine("================================\n");

                // Printing only for the height of the window, then clearing the screen
                for (int i = 0; i < Console.WindowHeight - 5; i++)
                {
                    Console.WriteLine(Ping(address, buffer, timeout, ttl));
                    Thread.Sleep(100);
                }
                Console.Clear();
            }
        }

        // Ping function used for sending ICMP packets
        public static string Ping(string _address, byte[] buffer, int timeout, int ttl)
        {
            // Exceptions
            if (!IPAddress.TryParse(_address, out IPAddress address) || address.GetAddressBytes()[0] == 0)
            {
                if (_address.Any(x => char.IsLetter(x)))
                {
                    try
                    {
                        address = Dns.GetHostAddresses(_address).First();
                    }
                    catch
                    {
                        return "The adress is invalid.";
                    }
                }
                else
                {
                    return "The adress is invalid.";
                }
            }
            if (buffer.Length <= 0)
            {
                return "Buffer size has to be positive.";
            }
            else if (buffer.Length > 65500)
            {
                return "The buffer size cannot exceed 65500 bytes.";
            }
            if (timeout <= 0)
            {
                return "Timeout has to be positive.";
            }
            if (ttl <= 0)
            {
                return "TTL has to be positive.";
            }

            // Define the pinger and its options
            Ping pinger = new Ping();
            PingOptions options = new PingOptions(ttl, true);

            // Try to send the ICMP packet and return the appropriate string
            try
            {
                PingReply reply = pinger.Send(address, timeout, buffer, options);
                return reply.Status switch
                {
                    IPStatus.Success => $"Reply from {reply.Address}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms TTL={reply.Options.Ttl}",

                    IPStatus.TimedOut => $"Reply from {reply.Address}: Offline",

                    IPStatus.DestinationHostUnreachable => "Desitnation host unreachable.",

                    IPStatus.PacketTooBig => $"Reply from {reply.Address}: Packet too big",

                    _ => $"Something broke!\n{reply.Status}"
                };
            }
            catch (Exception ex)
            {
                return $"Something broke!\n{ex}";
            }
        }

        // multiply function used for creating the byte array
        public static string Multiply(string str, int mulitplier)
        {
            string rstring = "";
            for (int i = 0; i < mulitplier; i++)
            {
                rstring = rstring + str;
            }
            return rstring;
        }
    }
}