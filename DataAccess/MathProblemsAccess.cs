using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Entities;
using MySql.Data.MySqlClient;
using System.IO;

namespace DataAccess
{
    public class MathProblemsAccess
    {
        public bool AddMathProblem(MySqlConnection conn,MathProblem mathProblem)
        {
            bool isScucceed = false;
            try
            {
                var isWriteSucceed =WriteProblemFile(mathProblem);
                if (isWriteSucceed)
                {
                    mathProblem.Problem = "已存"; 
                }
                using (conn)
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    string insertProblemStr = "insert into mathproblems values(@ID,@Type,@Grade,@Chapter,@Knowledges,@Publisher,@Problem,@Graphic,@State)";

                    MySqlCommand command = new MySqlCommand(insertProblemStr, conn);
                    command.Parameters.AddWithValue("@ID", mathProblem.ID);
                    command.Parameters.AddWithValue("@Type", mathProblem.Type);
                    command.Parameters.AddWithValue("@Grade", mathProblem.Grade);
                    command.Parameters.AddWithValue("@Chapter", mathProblem.Chapter);
                    command.Parameters.AddWithValue("@Knowledges", mathProblem.Knowledges);
                    command.Parameters.AddWithValue("@Publisher", mathProblem.Publisher);
                    command.Parameters.AddWithValue("@Problem", mathProblem.Problem);
                    command.Parameters.AddWithValue("@Graphic", mathProblem.Graphic);
                    command.Parameters.AddWithValue("@State", 1);

                    int result = command.ExecuteNonQuery();
                    isScucceed = result == 1 ? true : false;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                isScucceed = false;                
            }

            return isScucceed;
        }

        public string GetMathProblems(string queryStr, MySqlConnection conn)
        {
            string mathProblemsJson = string.Empty;
            try
            {
                using (conn)
                {
                    conn.Open();

                    List<MathProblem> mathProblems = new List<MathProblem>();
                    MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MathProblem problem = new MathProblem();
                            problem.ID = reader["ID"].ToString();
                            problem.Type = reader["Type"].ToString();
                            problem.Grade = reader["Grade"].ToString();
                            problem.Chapter = reader["Chapter"].ToString();
                            problem.Knowledges = reader["Knowledges"].ToString();
                            problem.Publisher = reader["Publisher"].ToString();
                            mathProblems.Add(problem);
                        }
                    }
                    mathProblemsJson= Newtonsoft.Json.JsonConvert.SerializeObject(mathProblems);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }        

            return mathProblemsJson;
        }

        private static bool WriteProblemFile(MathProblem mathProblem)
        {
            bool isSucceed = false;
            string mathPath = @"D:\StudentCenter\Math";
            string mathProblemFilePath = Path.Combine(mathPath, mathProblem.ID + ".txt");
            try
            {
                using (FileStream fileStrem = new FileStream(mathProblemFilePath, FileMode.Create))
                {
                    if (!string.IsNullOrEmpty(mathProblem.Problem))
                    {
                        var bytes = System.Text.Encoding.Unicode.GetBytes(mathProblem.Problem);
                        fileStrem.Write(bytes, 0, mathProblem.Problem.Length);
                        isSucceed = true;
                    }
                }

            }
            catch (Exception ex)
            {
                isSucceed = false;
              
            }

            return isSucceed;
        }

     

    }
}
