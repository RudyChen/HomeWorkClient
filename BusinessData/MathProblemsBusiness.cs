using DataAccess;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace BusinessData
{
    public class MathProblemsBusiness
    {

        string connStr = ConfigurationManager.ConnectionStrings["MathProbelmDbConnection"].ConnectionString.ToString();
        MySqlConnection mysqlConn;
        public MathProblemsBusiness()
        {
            mysqlConn = new MySqlConnection(connStr);
        }

        public bool AddMathProblem(MathProblem problem)
        {
            bool isSucceed = false;

            try
            {
                MathProblemsAccess access = new MathProblemsAccess();
                isSucceed= access.AddMathProblem(mysqlConn, problem);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return isSucceed;
        }

        public string GetMathProblems(string queryParametersJson)
        {
            string mathProblemsJson = string.Empty;

            var mathProblem = Newtonsoft.Json.JsonConvert.DeserializeObject<MathProblem>(queryParametersJson);

            string conditionStr = string.Empty;
            var queryParameters = CreateQueryCondition(mathProblem, ref conditionStr);
            if (string.IsNullOrEmpty(conditionStr))
            {
                return "no condition found";
            }

            string queryStr = "select * from mathproblems " + conditionStr;
            try
            {
                MathProblemsAccess access = new MathProblemsAccess();
                mathProblemsJson = access.GetMathProblems(queryStr,mysqlConn);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return mathProblemsJson;
        }

        private List<MySqlParameter> CreateQueryCondition(MathProblem mathProblem, ref string conditionStr)
        {

            List<MySqlParameter> parameterArray = new List<MySqlParameter>();
            StringBuilder conditionBuilder = new StringBuilder();
            conditionBuilder.Append("where ");
            if (string.IsNullOrEmpty(mathProblem.Type)
                && string.IsNullOrEmpty(mathProblem.Grade)
                && string.IsNullOrEmpty(mathProblem.Chapter)
                && string.IsNullOrEmpty(mathProblem.Knowledges)
                && string.IsNullOrEmpty(mathProblem.Publisher))
            {
                conditionStr = string.Empty;
                return null;
            }

            if (!string.IsNullOrEmpty(mathProblem.Type))
            {
                conditionBuilder.Append("Type=@Type");
                MySqlParameter parameter = new MySqlParameter("@Type", mathProblem.Type);
                parameterArray.Add(parameter);


            }
            if (!string.IsNullOrEmpty(mathProblem.Grade))
            {
                if (parameterArray.Count > 0)
                {
                    conditionBuilder.Append(" AND Grade=@Grade");
                }
                else
                {
                    conditionBuilder.Append("Grade=@Grade");
                }

                MySqlParameter parameter = new MySqlParameter("@Grade", mathProblem.Type);
                parameterArray.Add(parameter);
            }
            if (!string.IsNullOrEmpty(mathProblem.Chapter))
            {
                if (parameterArray.Count > 0)
                {
                    conditionBuilder.Append(" AND Chapter=@Chapter");
                }
                else
                {
                    conditionBuilder.Append("Chapter=@Chapter");
                }

                MySqlParameter parameter = new MySqlParameter("@Chapter", mathProblem.Type);
                parameterArray.Add(parameter);
            }
            if (!string.IsNullOrEmpty(mathProblem.Knowledges))
            {
                if (parameterArray.Count > 0)
                {
                    conditionBuilder.Append(" AND Knowledges=@Knowledges");
                }
                else
                {
                    conditionBuilder.Append("Knowledges=@Knowledges");
                }

                MySqlParameter parameter = new MySqlParameter("@Knowledges", mathProblem.Type);
                parameterArray.Add(parameter);
            }
            if (!string.IsNullOrEmpty(mathProblem.Publisher))
            {
                if (parameterArray.Count > 0)
                {
                    conditionBuilder.Append(" AND Publisher=@Publisher");
                }
                else
                {
                    conditionBuilder.Append("Publisher=@Publisher");
                }

                MySqlParameter parameter = new MySqlParameter("@Publisher", mathProblem.Type);
                parameterArray.Add(parameter);
            }



            return parameterArray;
        }
    }
}
