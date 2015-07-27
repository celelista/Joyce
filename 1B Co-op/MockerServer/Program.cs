using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MockerServer;

/*References:
 * http://www.codeproject.com/Articles/488668/Csharp-TCP-Server
 *http://www.codeproject.com/Articles/10649/An-Introduction-to-Socket-Programming-in-NET-using
 * http://www.codeproject.com/Articles/5733/A-TCP-IP-Server-written-in-C
 * http://www.csharp-examples.net/socket-send-receive/
 * http://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
 */

namespace NodeCom_Test
{

	class MockServer
	{
		private static IPAddress longIP;
		private static TcpListener listener;

		static void Main()
		{
			ServerCreator();
		}

		private static void ServerCreator()
		{
			//80 is the http port number, 24.246.62.251 is our own IP address, 8.8.8.8 is the Google server IP address
			//conversion of IP address to a long
			longIP = IPAddress.Parse("127.0.0.1");
			listener = new TcpListener(longIP, 80);
			int requestCount = 1;

			//start the server
			listener.Start();
			Console.WriteLine("Server started");
			TcpClient client = listener.AcceptTcpClient();
			Console.WriteLine("Accept connection from client");

			//keeping track of the number of requests
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("================================<<Client Request " + requestCount + ">>========================================");
			requestCount++;

			//Get client stream
			var networkStream = client.GetStream();
		
			//Getting the data package in a byte array
			var bytesFrom = GetPackage(client, networkStream);

			//Check the checksum
			var receivedChecksum = BitConverter.ToUInt16(bytesFrom, bytesFrom.Length - 2);
			var calculatedChecksum = CrcGen(bytesFrom.Take(bytesFrom.Length - 2).ToArray());

			byte[] confirmation;
			byte[] getResponse;

			if (receivedChecksum == calculatedChecksum)
			{
				Console.WriteLine("Checksum OK");

				// Message about the data
				string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
				Console.WriteLine("Data from client - " + dataFromClient);
			}
			else
			{
				Console.WriteLine("Checksum Mismatch");

				//sending back NAK for a bad crc
				getResponse = new byte[] {0x7e, 0x15, 0x7e};

				//send back response
				networkStream.Write(getResponse, 0, getResponse.Length);
			
			}

			//Run tests on the package information to ensure that client error-checking is working
			//ServerTests(ref bytesFrom);

			// The command class is not recognized
			if ((bytesFrom[5] != 'S') && (bytesFrom[5] != 'G'))
			{
				//sending back BS response
				getResponse = new byte[] {0x7e, 0x08, 0x7e};
				networkStream.Write(getResponse, 0, getResponse.Length);
			}

			//'SET' data command
			else if ((char)bytesFrom[5] == 'S')
			{
				//figure out command type
				ProcessSetCommand(bytesFrom, out confirmation);

				//command is not valid
				if (confirmation.Length == 0)
				{
					//command was not valid, sending back BS
					getResponse = new byte[] { 0x7e, 0x08, 0x7e };
					//send back a response
					networkStream.Write(getResponse, 0, getResponse.Length);
					networkStream.Flush();
				}
				//command is valid, send back ACK
				else
				{
					//send back a response
					networkStream.Write(confirmation, 0, confirmation.Length);
					networkStream.Flush();
				}
			}

			//"Get" data command
			else if ((char)bytesFrom[5] == 'G')
			{
				char[] commands = new[] {'G', 'M', 'I', 'L', 'f', 'i', 'C', 'O', 'T', 'U', 'N', 'S', 'X'};	

				//check to see if the command was valid
				if (commands.Contains((char)bytesFrom[6]))
				{
					//command was not valid, sending back BS
					confirmation = new byte[] { 0x7e, 0x08, 0x7e };
					//send back a response
					networkStream.Write(confirmation, 0, 3);
					networkStream.Flush();
				} else
				{
					// command was valid, send back ENQ. Await response package
					confirmation = new byte[] { 0x7e, 0x05, 0x7e };
					
					//sending ENQ
					networkStream.Write(confirmation, 0, 3);
					networkStream.Flush();
					
					//send data package
					ProcessGetCommand(bytesFrom, client);
				}
			}
		}


		/// <summary>
		/// Gets the data package and then unpacks it, returning the processed package
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		private static byte[] GetPackage(TcpClient client, NetworkStream networkStream)
		{
			//Receive a full packet
			var packetData = new List<byte>();
			var parsingPacket = false;

			while (true)
			{
				var buffer = new byte[1024];

				//Receive data
				var bytesReceived = networkStream.Read(buffer, 0, buffer.Length);

				//Zero bytes received means client disconnected
				if (bytesReceived == 0)
				{
					Console.WriteLine("The client has disconnected");
				}

				//Examine received bytes
				for (var i = 0; i < bytesReceived; i++)
				{
					if (buffer[i] == 0x7e)
					{
						if (parsingPacket)
						{
							//Packet is done
							packetData.Add(buffer[i]);
							parsingPacket = false;

							//Decode the packet
							var decoded = HdlcDecode(packetData.ToArray());

							return decoded;
						} else
						{
							parsingPacket = true;
							packetData = new List<byte> { buffer[i] };
						}
						continue;
					}

					if (parsingPacket)
						packetData.Add(buffer[i]);
				}

			}
		}
			
		/// <summary>
		///Generates CRC from the packet data  
		/// </summary>
		public static ushort CrcGen(byte[] chbuff)
		{
			return CrcAdd(chbuff, 0xffff);
		}

		private static ushort CrcAdd(byte[] buff, ushort initVal)
		{
			byte crcLow = (byte)initVal;
			byte crcHi = (byte)(initVal >> 8);

			for (int i = 0; i < buff.Length; i++)
			{
				var index = (crcHi ^ buff[i]);

				crcHi = (byte)(crcLow ^ crcTabHi[index]);
				crcLow = crcTabLow[index];
			}

			ushort crc = (ushort)(crcHi << 8 | crcLow);

			return crc;
		}

		private static byte[] crcTabHi = {
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 0 - 7
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 8 - 15
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 16 - 23
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 24 - 31
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 32 - 39
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 40 - 47
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 48 - 55
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 56 - 63
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 64 - 71
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 72 - 79
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 80 - 87
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 88 - 95
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 96 - 103
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 104 - 112
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 113 - 119
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 120 - 127
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 128 - 135
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 136 - 143
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 144 - 151
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 152 - 159
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 160 - 167
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 168 - 175
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 176 - 183
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 184 - 191
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 192 - 199
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 200 - 207
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 208 - 215
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 216 - 223
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, // 224 - 231
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 232 - 239
                        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, // 240 - 247
                        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40  // 248 - 255
                };

		private static byte[] crcTabLow = {
                        0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2,			// 0 - 7
                        0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04,         // 8 - 15
                        0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,         // 16 - 23
                        0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8,         // 24 - 31
                        0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,         // 32 - 39
                        0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,         // 40 - 47
                        0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6,         // 48 - 55
                        0xD2, 0x12, 0x13, 0xD3, 0x11, 0xD1, 0xD0, 0x10,         // 56 - 63
                        0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,         // 64 - 71
                        0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,         // 72 - 79
                        0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE,         // 80 - 87
                        0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,         // 88 - 95
                        0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA,         // 96 - 103
                        0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C,         // 104 - 112
                        0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,         // 113 - 119
                        0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0,         // 120 - 127
                        0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62,         // 128 - 135
                        0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,         // 136 - 143
                        0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE,         // 144 - 151
                        0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,         // 152 - 159
                        0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,         // 160 - 167
                        0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C,         // 168 - 175
                        0xB4, 0x74, 0x75, 0xB5, 0x77, 0xB7, 0xB6, 0x76,         // 176 - 183
                        0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,         // 184 - 191
                        0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,         // 192 - 199
                        0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54,         // 200 - 207
                        0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,         // 208 - 215
                        0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98,         // 216 - 223
                        0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A,         // 224 - 231
                        0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,         // 232 - 239
                        0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86,         // 240 - 247
                        0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80, 0x40			// 248 - 255
                };

		/// <summary>
		/// Takes in a string message and converts it to a byte[]
		/// </summary>
		/// <param name="message"></param>
		/// <returns> a byte[] that represents the message for further processing </returns>
		private static byte[] CreateByteMessage(string message)
		{
			byte[] byteMessage = Encoding.ASCII.GetBytes(message);
			return byteMessage;
		}
		/// <summary>
		/// Print a given packet message
		/// </summary>
		private static void PrintPacket(byte[] packet)
		{
			//Conversion of packet into a char array
			char[] message = new char[packet.Length];

			for (int i = 0; i < packet.Length; i++)
			{
				message[i] = (char)packet[i];
			}

			//Conversion to string
			string message1 = new string(message);

			// Clearing out unwanted chars
			Regex rgx = new Regex(@"[^a-zA-Z0-9 \p{P}]");
			string s1 = rgx.Replace(message1, ".");

			//Conversion back to char array
			message = s1.ToCharArray();


			//creating spacing between messages
			Console.WriteLine();
			Console.WriteLine();

			//Formatting the string nicely with lines
			for (int j = 0; j < s1.Length; j += 16)
			{
				//case: it is near the end of the string
				if ((s1.Length - j) <= 16)
				{
					Console.Write("<- [");
					Console.Write(s1.Substring(j, (s1.Length - j)));
					Console.WriteLine("]");
					break;
				}
					//for every 16 chars, make a new line
				else if ((j % 16 == 0) && ((j + 16) < s1.Length))
				{
					Console.Write("<- [");

					//writing out 16 chars
					for (int k = 0 + j; k < 16 + j; k++)
					{
						Console.Write(message[k]);
					}
					Console.WriteLine("]");
				}
			}
		}


		/// <summary>
		/// processing 'GET' class commands
		/// the package in this case is assumed to have gotten rid of the modified HDLC framing layer
		/// </summary>
		private static void ProcessGetCommand(byte[] package, TcpClient client)
		{
			//figuring out command type
			char type = (char)package[6];

			//creating an array holding the data for easier command processing
			byte[] data = new byte[package.Length - 9];
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = package[i + 7];
			}
			switch (type)
			{
				//check to see if command is valid
				case 'G': //Necessary
					GetCommandProcesser.GetGraphicFormatFrameInfo(package, client);
					break;
				case 'M': //Necessary
					GetCommandProcesser.GetFrameDataInOriginalFormat(package, client);
					break;
				case 'I': //Nice
					GetCommandProcesser.GetIntensityData(package, client);
					break;
				case 'f': //Good
					GetCommandProcesser.GetFontData(package, client);
					break;
				case 'C': //Good
					GetCommandProcesser.GetConfigurationData(package, client);
					break;
				case 'T': //Nice
					GetCommandProcesser.GetTimeData(package, client);
					break;
				case 'U': //Good
					GetCommandProcesser.GetUnitVersionData(package, client);
					break;
				case 'O':
				case 'i':
				case 'N':
				case 'S':
				case 'X':
				case 'L':
					break;
			}
		}

		/// <summary>
		/// processing 'SET' class commands
		/// </summary>
		private static void ProcessSetCommand(byte[] package, out byte[] response)
		{
			//check the command is indeed in the 'SET' class of commands
			if ((char)package[5] != 'S')
			{
				response = new byte[0];
				return;
			}
			
			//figuring out command type
			char type = (char)package[6];
			
			switch (type)
			{
				//check to see if command was valid
				case 'B':
				case 'D':
				case 'E':
				case 'I':
				case 'R':
				case 'M':
				case 'f':
				case 'i':
				case 'C':
				case 'F':
				case 'O':
				case 'T':
				case 'Z':
				case 'P':
				case 'Q':
				case 'X':
				case 'S':
					//The 'ACK' package
					response = new byte[] { 0x7e, 0x06, 0x7e };
					break;
				default:
					response = new byte[0];
					break;
			}
		}



		/// <summary>
		/// Hdlc decode a given packet. 
		/// </summary>
		/// <param name="packet">The packet to decode.</param>
		/// <returns>The decoded packet.</returns>
		public static byte[] HdlcDecode(byte[] packet)
		{
			if (packet == null)
				throw new ArgumentNullException("packet");

			//Make sure the packet is big enough
			if (packet.Length < 7) //Header, checksum, footer
				throw new ArgumentException("Packet is too small");

			//Make sure packet is properly delimited
			if (packet[0] != 0x7e || packet[packet.Length - 1] != 0x7e)
				throw new ArgumentException("Packet is not properly delimited");

			//Unescape the packet content (remove byte stuffing)
			byte[] unescapedContent;
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				var escaping = false;

				//Iterate over all bytes except for the first and last
				for (var i = 1; i < packet.Length - 1; i++)
				{
					var currByte = packet[i];

					//Check if we are escaping
					if (escaping)
					{
						if (currByte == 0x5e)
							bw.Write((byte)0x7e);
						else if (currByte == 0x5d)
							bw.Write((byte)0x7d);
						else
							throw new ArgumentException("Invalid escape sequence");

						escaping = false;
						continue;
					}

					//Check if this is an escape character
					if (currByte == 0x7d)
					{
						escaping = true;
						continue;
					}

					bw.Write(currByte);
				}

				unescapedContent = ms.ToArray();
			}

			//Return the packet payload
			return unescapedContent;
		}
	}
}
