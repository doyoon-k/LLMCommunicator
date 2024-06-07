// See https://aka.ms/new-console-template for more information
using OllamaCSDemo;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        OllamaCommunicator ollaCommunicator = new OllamaCommunicator("llama3","You are a virtual assistant who gives me whatever answer I want.");

        //get input from user until user types "exit"
        string input = "";
        while (input != "exit")
        {
            Console.WriteLine("Enter a prompt or type 'exit' to quit:");
            input = Console.ReadLine();
            if (input != "exit")
            {
                string response = await ollaCommunicator.GenerateCompletionAsync("llama3", input);
                Console.WriteLine(response);
                Console.WriteLine();
            }
        }
    }
}



