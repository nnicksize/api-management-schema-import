﻿using Microsoft.Azure.ApiManagement.WsdlProcessor.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.ApiManagement.WsdlProcessor.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string wsdlFile, outputFile = string.Empty;

            var log = new ConsoleLog();
            int exitCode = 0;

            if (args.Length == 2)
            {
                wsdlFile = args[0];
                if (!File.Exists(wsdlFile))
                {
                    var msg = $"The input file {wsdlFile} does not exists or we cannot access.";
                    log.Error(msg);
                    throw new Exception(msg);
                }

                wsdlFile = wsdlFile.Contains(".wsdl") ? wsdlFile : wsdlFile + ".wsdl";

                outputFile = args[1];
                outputFile = Path.IsPathRooted(outputFile) ? outputFile : Path.Join(Directory.GetCurrentDirectory(), outputFile);
            }
            else
            {
                Console.WriteLine("Please fill the corrects parameters.");
                Console.WriteLine("Parameters 1 = WSDL file path.");
                Console.WriteLine("Parameters 2 = Output file path.");
                Environment.Exit(1);

                return;
            }

            try
            {
                var wsdlString = File.ReadAllText(wsdlFile);
                var xDocument = XDocument.Parse(wsdlString);
                await WsdlDocument.LoadAsync(xDocument.Root, log);

                xDocument.Root.Save(outputFile);

                Console.WriteLine();
                Console.WriteLine();

                log.Success($"*** WSDL file {wsdlFile} processed successfully and generate file {outputFile}.");
            }
            catch (XmlException e)
            {
                exitCode = 1;
                log.Error($"{e.Message}");
                log.Error($"Location {wsdlFile} contains invalid xml: {e.StackTrace}");
            }
            catch (Exception e)
            {
                exitCode = 1;
                log.Error($"{e.Message}");
                log.Error($"Stacktrace: {e.StackTrace}");
            }

            Environment.Exit(exitCode);
        }
    }
}