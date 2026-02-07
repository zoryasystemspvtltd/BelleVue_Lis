using LIS.DtoModel.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIS.Com.Businesslogic
{
    public static class LisExtension
    {
        public static string GetValue(this string message, int start, int length)
        {
            length = message.Length < (start + length) ? (message.Length - start) : length;
            string value = message.Substring(start, length);
            return value;
        }
    }
    public class DxC700Request
    {
        protected readonly DxC700Configuration configuration;

        public DxC700Request(DxC700Configuration configuration)
        {
            this.configuration = configuration;
        }

        public string RackNo { get; set; } = string.Empty;

        public string CupNo { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string SampleNo { get; set; } = string.Empty;

        public string SampleId { get; set; } = string.Empty;

        public void ProcessMessage(string message)
        {
            int start = 2; // First two digit is command name
            int length = 0;
            if (configuration.RackNo.Enabled)
            {
                length = configuration.RackNo.Lenght;
                this.RackNo = message.GetValue(start, length).Trim();
                start += configuration.RackNo.Lenght;
            }

            if (configuration.CupNo.Enabled)
            {
                length = configuration.CupNo.Lenght;
                this.CupNo = message.GetValue(start, length).Trim();
                start += configuration.CupNo.Lenght;
            }

            if (configuration.Type.Enabled)
            {
                length = configuration.Type.Lenght;
                this.Type = message.GetValue(start, length).Trim();
                start += configuration.Type.Lenght;
            }

            if (configuration.SampleNo.Enabled)
            {
                length = configuration.SampleNo.Lenght;
                this.SampleNo = message.GetValue(start, length).Trim();
                start += configuration.SampleNo.Lenght;
            }

            if (configuration.SampleId.Enabled)
            {
                length = configuration.SampleId.Lenght;
                this.SampleId = message.GetValue(start, length).Trim(); // *26 digit sample id (Bar-code Number)
                start += configuration.SampleNo.Lenght;
            }
        }
    }

    public class DxC700Result : Result
    {
        protected readonly DxC700Configuration configuration;

        public DxC700Result(DxC700Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void ProcessMessage(string message)
        {
            int start = 2; // First two digit is command name
            int length = 0;

            TestResult testResult = new TestResult
            {
                ResultDate = DateTime.Now
            };

            if (configuration.RackNo.Enabled)
            {
                start += configuration.RackNo.Lenght;
            }

            if (configuration.CupNo.Enabled)
            {
                start += configuration.CupNo.Lenght;
            }

            if (configuration.Type.Enabled)
            {
                start += configuration.Type.Lenght;
            }

            if (configuration.SampleNo.Enabled)
            {
                start += configuration.SampleNo.Lenght;
            }

            if (configuration.SampleId.Enabled)
            {
                length = configuration.SampleId.Lenght;
                testResult.SampleNo = message.GetValue(start, length).Trim();
                start += configuration.SampleId.Lenght;
            }

            if (configuration.Dummy.Enabled)
            {
                start += configuration.Dummy.Lenght;
            }

            if (configuration.BlockIdNo.Enabled)
            {
                start += configuration.BlockIdNo.Lenght;
            }
            if (configuration.Sex.Enabled)
            {
                start += configuration.Sex.Lenght;
            }
            if (configuration.Year.Enabled)
            {
                start += configuration.Year.Lenght;
            }
            if (configuration.Month.Enabled)
            {
                start += configuration.Month.Lenght;
            }
            if (configuration.OtherType.Enabled)
            {
                start += configuration.OtherType.Lenght;
            }
            if (configuration.PatientInfo.Enabled)
            {
                start += configuration.PatientInfo.Lenght;
            }
            if (configuration.PatientId.Enabled)
            {
                start += configuration.PatientId.Lenght;
            }

            if (configuration.RunDateTime.Enabled)
            {
                start += configuration.RunDateTime.Lenght;
            }
            if (configuration.OnlineTestNumber.Enabled)
            {
                start += configuration.OnlineTestNumber.Lenght;
            }
            if (configuration.DilutionInfo.Enabled)
            {
                start += configuration.DilutionInfo.Lenght;
            }
            if (configuration.ReagentInfo.Enabled)
            {
                start += configuration.ReagentInfo.Lenght;
            }
            // Processing Test Parameter
            List<TestResultDetails> lsResult = new List<TestResultDetails>();
            var resultLength = configuration.ParamCode.Lenght + configuration.ParamValue.Lenght;
            bool hasResult = message.Length >= start + resultLength;
            while (hasResult)
            {
                TestResultDetails resultDetails = new TestResultDetails();

                // Enable checking not used here
                length = configuration.ParamCode.Lenght;
                resultDetails.LISParamCode = message.GetValue(start, length).Trim();
                start += configuration.ParamCode.Lenght;

                length = configuration.ParamValue.Lenght;
                resultDetails.LISParamValue = message.GetValue(start, length).Trim();
                start += configuration.ParamValue.Lenght;

                if (configuration.Flags.Enabled)
                {
                    start += configuration.Flags.Lenght;
                }
                if (configuration.ISEElectrode.Enabled)
                {
                    start += configuration.ISEElectrode.Lenght;
                }

                lsResult.Add(resultDetails);
                hasResult = message.Length >= start + resultLength;

                testResult.LISTestCode = resultDetails.LISParamCode;
            }


            this.TestResult = testResult;
            this.ResultDetails = lsResult;
        }
    }

    public class DxC700ControlResult : Result
    {
        protected readonly DxC700Configuration configuration;

        public DxC700ControlResult(DxC700Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void ProcessMessage(string message)
        {
            int start = 2; // First two digit is command name
            int length = 0;

            TestResult testResult = new TestResult
            {
                ResultDate = DateTime.Now
            };

            if (configuration.RackNo.Enabled)
            {
                start += configuration.RackNo.Lenght;
            }

            if (configuration.CupNo.Enabled)
            {
                start += configuration.CupNo.Lenght;
            }

            if (configuration.Type.Enabled)
            {
                start += configuration.Type.Lenght;
            }

            if (configuration.SampleNo.Enabled)
            {
                start += configuration.SampleNo.Lenght;
            }

            if (configuration.SampleId.Enabled)
            {
                length = configuration.SampleId.Lenght;
                testResult.SampleNo = message.GetValue(start, length).Trim();
                start += configuration.SampleId.Lenght;
            }

            // For Control Result DUMMY NOT in use
            //if (configuration.Dummy.Enabled)
            //{
            //    start += configuration.Dummy.Lenght;
            //}

            if (configuration.ControlNo.Enabled)
            {
                start += configuration.ControlNo.Lenght;
            }
            if (configuration.BlockIdNo.Enabled)
            {
                start += configuration.BlockIdNo.Lenght;
            }
            if (configuration.RunDateTime.Enabled)
            {
                start += configuration.RunDateTime.Lenght;
            }
            if (configuration.OnlineTestNumber.Enabled)
            {
                start += configuration.OnlineTestNumber.Lenght;
            }
            if (configuration.DilutionInfo.Enabled)
            {
                start += configuration.DilutionInfo.Lenght;
            }
            if (configuration.ReagentInfo.Enabled)
            {
                start += configuration.ReagentInfo.Lenght;
            }
            // Processing Test Parameter
            List<TestResultDetails> lsResult = new List<TestResultDetails>();
            var resultLength = configuration.ParamCode.Lenght + configuration.ParamValue.Lenght;
            bool hasResult = message.Length >= start + resultLength;
            while (hasResult)
            {
                TestResultDetails resultDetails = new TestResultDetails();

                // Enable checking not used here
                length = configuration.ParamCode.Lenght;
                resultDetails.LISParamCode = message.GetValue(start, length).Trim();
                start += configuration.ParamCode.Lenght;

                length = configuration.ParamValue.Lenght;
                resultDetails.LISParamValue = message.GetValue(start, length).Trim();
                start += configuration.ParamValue.Lenght;

                if (configuration.Flags.Enabled)
                {
                    start += configuration.Flags.Lenght;
                }
                if (configuration.ISEElectrode.Enabled)
                {
                    start += configuration.ISEElectrode.Lenght;
                }

                lsResult.Add(resultDetails);
                hasResult = message.Length >= start + resultLength;
            }


            this.TestResult = testResult;
            this.ResultDetails = lsResult;
        }
    }
    public class DxC700Response : DxC700Request
    {
        public DxC700Response(DxC700Configuration configuration)
            : base(configuration)
        {

        }

        public string Dummy { get; set; } = string.Empty;

        public string BlockIdNo { get; set; } = string.Empty;

        public string Sex { get; set; } = string.Empty;

        public string Year { get; set; } = string.Empty;

        public string Month { get; set; } = string.Empty;

        public string OtherType { get; set; } = string.Empty;

        public string PatientInfo { get; set; } = string.Empty;

        public string PatientId { get; set; } = string.Empty;

        public List<Dxc700Tests> Tests { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("S ");

            if (configuration.RackNo.Enabled)
            {
                builder.Append(RackNo.PadLeft(configuration.RackNo.Lenght, ' '));
            }

            if (configuration.CupNo.Enabled)
            {
                builder.Append(CupNo.PadLeft(configuration.CupNo.Lenght, ' '));
            }

            if (configuration.Type.Enabled)
            {
                builder.Append(Type.PadLeft(configuration.Type.Lenght, ' '));
            }

            if (configuration.SampleNo.Enabled)
            {
                builder.Append(SampleNo.PadLeft(configuration.SampleNo.Lenght, ' '));
            }

            if (configuration.SampleId.Enabled)
            {
                builder.Append(SampleId.PadLeft(configuration.SampleId.Lenght, ' '));
            }

            if (configuration.Dummy.Enabled)
            {
                builder.Append(Dummy.PadLeft(configuration.Dummy.Lenght, ' '));
            }

            if (configuration.BlockIdNo.Enabled)
            {
                builder.Append("E");
                //builder.Append(BlockIdNo.PadLeft(configuration.BlockIdNo.Lenght, ' '));
            }

            if (configuration.Sex.Enabled)
            {
                builder.Append(Sex.PadLeft(configuration.Sex.Lenght, ' '));
            }

            if (configuration.Year.Enabled)
            {
                builder.Append(Year.PadLeft(configuration.Year.Lenght, '0'));
            }

            if (configuration.Month.Enabled)
            {
                builder.Append(Month.PadLeft(configuration.Month.Lenght, '0'));
            }

            if (configuration.OtherType.Enabled)
            {
                builder.Append(OtherType.PadLeft(configuration.OtherType.Lenght, ' '));
            }

            if (configuration.PatientInfo.Enabled)
            {
                builder.Append(PatientInfo.PadRight(configuration.PatientInfo.Lenght, ' '));
            }

            if (configuration.PatientId.Enabled)
            {
                builder.Append(PatientId.PadRight(configuration.PatientId.Lenght, ' '));
            }

            foreach (var item in Tests)
            {
                builder.Append(item.ToString());
            }

            return builder.ToString();
        }


    }

    public class Dxc700Tests
    {
        private readonly DxC700Configuration configuration;

        public Dxc700Tests(DxC700Configuration configuration)
        {
            this.configuration = configuration;
        }

        [StringLength(3)]
        public string TestNo { get; set; } = string.Empty;

        [StringLength(1)]
        public string DilInfo { get; set; } = string.Empty;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (configuration.TestNo.Enabled)
            {
                builder.Append(TestNo.PadLeft(configuration.TestNo.Lenght, ' '));
            }

            if (configuration.DilInfo.Enabled)
            {
                builder.Append(DilInfo.PadLeft(configuration.DilInfo.Lenght, ' '));
            }
            return builder.ToString();
        }
    }

    public class DxC700Configuration
    {
        #region Request and Result Fields - Fixed Length
        public Field RackNo { get; set; }

        public Field CupNo { get; set; }

        public Field Type { get; set; }

        public Field SampleNo { get; set; }

        public Field SampleId { get; set; }

        public Field Dummy { get; set; }

        public Field BlockIdNo { get; set; }

        public Field Sex { get; set; }

        public Field Year { get; set; }

        public Field Month { get; set; }

        public Field OtherType { get; set; }

        public Field PatientInfo { get; set; }

        public Field PatientId { get; set; }

        public Field ControlNo { get; set; }
        #endregion

        #region Result Parameter - Variable 
        public Field RunDateTime { get; set; }

        public Field OnlineTestNumber { get; set; }

        public Field DilutionInfo { get; set; }

        public Field ReagentInfo { get; set; }

        public Field ParamCode { get; set; }

        public Field ParamValue { get; set; }

        public Field Flags { get; set; }

        public Field ISEElectrode { get; set; }
        #endregion

        #region Test Field - Variable 
        public Field TestNo { get; set; }

        public Field DilInfo { get; set; }
        #endregion
    }

    public class Field
    {
        public int Lenght { get; set; }

        public bool Enabled { get; set; }
    }
}
