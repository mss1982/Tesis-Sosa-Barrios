﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVM_Multiclass_Interface
{
    public class SVMMulticlass : ISVMMulticlass
    {
        public string Learn(string example_file, string model_file, string c, ref int code)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            string[] lines = File.ReadAllLines(example_file);
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo("svm_bin/svm_multiclass_learn.exe", String.Format("-c {0} \"{1}\" \"{2}\"", c, example_file, model_file));
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.RedirectStandardError = true;
            p.StartInfo = myProcessStartInfo;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit();
            code = p.ExitCode;
            p.Close();
            return output;
        }

        public string Classify(string example_file, string model_file, string predictions_file, ref int code)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            string[] lines = File.ReadAllLines(example_file);
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo("svm_bin/svm_multiclass_classify.exe", String.Format("\"{0}\" \"{1}\" \"{2}\"", example_file, model_file, predictions_file));
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            p.StartInfo = myProcessStartInfo;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            code = p.ExitCode;
            p.Close();
            return output;
        }
    }
}
