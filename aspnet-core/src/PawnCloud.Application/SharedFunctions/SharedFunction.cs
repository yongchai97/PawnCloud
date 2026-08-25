using PawnCloud.GeneralSetups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.SharedFunctions
{
    public class SharedFunction
    {
        public string GenerateTicketNumber(int currentTicketCount, GeneralSetup generalSetup, DateTime pledgedDate, string ticketNumberFromUserInput)
        {
            try
            {
                string ticketNumber = "";
                string startingAlphabets = "AAAA";
                // Append the appendedString if it is not null or empty
                if (!string.IsNullOrEmpty(generalSetup.appendedString))
                {
                    ticketNumber += generalSetup.appendedString;
                }
                // Append the year and month if AppendYearMonth is true
                if (generalSetup.AppendYearMonth)
                {
                    ticketNumber += pledgedDate.ToString("yyyyMM");
                }
                // Append the current ticket count
                switch(generalSetup.ticketIdMethod)
                {
                    case 1:
                        ticketNumber += (currentTicketCount + 1).ToString("D4"); // D4 formats the number to 4 digits with leading zeros
                        break;
                    case 2:
                        for(int i = 0; i<=currentTicketCount; i++)
                        {
                            startingAlphabets = IncrementCode(startingAlphabets);
                        }
                        ticketNumber += startingAlphabets;
                        break;
                    case 3:
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        string code = new string(
                                    Enumerable.Range(0, 4)
                                        .Select(_ => chars[Random.Shared.Next(chars.Length)])
                                        .ToArray()
                                );
                        ticketNumber += code;
                        break;
                    case 4:
                        ticketNumber = ticketNumberFromUserInput;
                        break;
                    case 5:
                        ticketNumber = GenerateCode(currentTicketCount, 10);
                        break;
                    default:
                        ticketNumber = ticketNumberFromUserInput;
                        break;
                }
                return ticketNumber;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use your preferred logging framework)
                Console.WriteLine($"Error generating ticket number: {ex.Message}");
            }
            return "";
        }
        private string IncrementCode(string code)
        {
            char[] chars = code.ToCharArray();

            for (int i = chars.Length - 1; i >= 0; i--)
            {
                if (chars[i] < 'Z')
                {
                    chars[i]++;
                    break;
                }

                chars[i] = 'A';
            }

            return new string(chars);
        }
        private string GenerateCode(int userValue, int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            byte[] randomBytes = RandomNumberGenerator.GetBytes(length);

            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                int index = (randomBytes[i] + userValue) % chars.Length;
                result[i] = chars[index];
            }

            return new string(result);
        }
    }
}
