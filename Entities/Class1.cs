using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MathProblem
    {
        private string id;

        private string type;

        private string grade;

        private string chapter;

        private string knowledges;

        private string publisher;

        private string problem;

        private string graphic;

        private int state;

        public int State
        {
            get { return state; }
            set { state = value; }
        }


        public string Graphic
        {
            get { return graphic; }
            set { graphic = value; }
        }


        public string Problem
        {
            get { return problem; }
            set { problem = value; }
        }


        public string Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }


        public string Knowledges
        {
            get { return knowledges; }
            set { knowledges = value; }
        }


        public string Chapter
        {
            get { return chapter; }
            set { chapter = value; }
        }


        public string Grade
        {
            get { return grade; }
            set { grade = value; }
        }


        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

    }
}
