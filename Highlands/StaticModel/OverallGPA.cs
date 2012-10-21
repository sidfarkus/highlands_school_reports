using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.StaticModel
{
    public class OverallGPA
    {        
        public double ComputeGPA()
        {            
            //bool bForgiven = false;
            int nCourseCount = 0;            
            double gpa = 0.0;   
                     
            gpa = gpa + GradePoints("A");
            nCourseCount++;           
            
            gpa = (nCourseCount > 0) ? gpa / nCourseCount : 0.0;
            
            return gpa;
        }

        public static double GradePoints(string grade)
        {
            switch (grade)
            {
                case "A+":
                    return 4.3;
                case "A":
                    return 4.0;                    
                case "A-":
                    return 3.6;

                case "B+":
                    return 3.3;
                case "B":
                    return 3.0;
                    
                case "B-":
                    return 2.6;
                    
                case "C+":
                    return 2.3;
                    
                case "C":
                    return 2.0;
                    
                case "C-":
                    return 1.6;
                    
                default:
                    return 0.0;
                    
            }
        }
    }
}
