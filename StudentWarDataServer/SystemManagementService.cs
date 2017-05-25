using BusinessData;
using Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace StudentWarDataServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class SystemManagementService : ISystemManagementService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string GetMathProblem(string queryParametersJson)
        {
            string mathproblemsJson = string.Empty;
            MathProblemsBusiness business = new MathProblemsBusiness();
            try
            {
                mathproblemsJson = business.GetMathProblems(queryParametersJson);
            }
            catch (Exception ex)
            {
                //Log:exception
                return string.Empty;  
            }

            return mathproblemsJson;
        }

  

        public bool InsertMathProblem(string mathProblemJson)
        {
            bool isSucceed = false;
            MathProblem problem = Newtonsoft.Json.JsonConvert.DeserializeObject<MathProblem>(mathProblemJson);
            try
            {
                MathProblemsBusiness business = new MathProblemsBusiness();
                isSucceed = business.AddMathProblem(problem);
            }
            catch (Exception ex)
            {
                isSucceed = false;
            }
            return isSucceed;
        }
    }
}
