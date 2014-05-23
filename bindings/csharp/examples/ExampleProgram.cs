using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iio;

namespace IIOCSharp
{
    class ExampleProgram
    {
        static void Main(string[] args)
        {
            Context ctx = new Context("10.44.2.241");
            if (ctx == null)
            {
                Console.WriteLine("Unable to create IIO context");
                return;
            }

            Console.WriteLine("IIO context created: " + ctx.name());

            Console.WriteLine("IIO context has " + ctx.get_devices().Count + " devices:");
            foreach (Device dev in ctx.get_devices()) {
                Console.WriteLine("\t" + dev.id() + ": " + dev.name());

                if (dev is Trigger)
                {
                    Console.WriteLine("Found trigger! Rate=" + ((Trigger) dev).get_rate());
                }

                Console.WriteLine("\t\t" + dev.get_channels().Count + " channels found:");

                foreach (Channel chn in dev.get_channels())
                {
                    string type = "input";
                    if (chn.is_output())
                        type = "output";
                    Console.WriteLine("\t\t\t" + chn.id() + ": " + chn.name() + " (" + type + ")");

                    if (chn.get_attrs().Count == 0)
                        continue;

                    Console.WriteLine("\t\t\t" + chn.get_attrs().Count + " channel-specific attributes found:");
                    foreach (Attr attr in chn.get_attrs())
                    {
                        Console.WriteLine("\t\t\t\t" + attr.name());
                        if (attr.name().CompareTo("frequency") == 0)
                        {
                            Console.WriteLine("Attribute content: " + attr.read());
                        }
                    }
                    
                }

				/* If we find cf-ad9361-lpc, try to read a few bytes from the first channel */
                if (dev.name().CompareTo("cf-ad9361-lpc") == 0)
                {
                    Channel chn = dev.get_channels()[0];
                    chn.enable();
                    IOBuffer buf = new IOBuffer(dev, 0x8000);
                    buf.refill();
                    
                    Console.WriteLine("Read " + chn.read(buf).Length + " bytes from hardware");
                    buf.Dispose();
                }

                if (dev.get_attrs().Count == 0)
                    continue;

                Console.WriteLine("\t\t" + dev.get_attrs().Count + " device-specific attributes found:");
                foreach (Attr attr in dev.get_attrs())
                    Console.WriteLine("\t\t\t" + attr.name());

            }

			/* Wait for user input */
            Console.ReadLine();
        }
    }
}
