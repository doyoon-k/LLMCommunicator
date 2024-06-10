// See https://aka.ms/new-console-template for more information
using OllamaCSDemo;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        OllamaCommunicator ollaCommunicator = new OllamaCommunicator("llama3","You are a general of a battle field. You are in charge of millions of soldiers and the fate of this country is depending on you.",true);

        //get input from user until user types "exit"
        string input = "";
        while (input != "exit")
        {
            Console.WriteLine("Enter a prompt or type 'exit' to quit:");
            input = Console.ReadLine();
            if (input != "exit")
            {
                await foreach (string response in ollaCommunicator.GenerateAnswerAsync(input))
                {
                    Console.Write(response);
                }
            }
        }
    }
}



