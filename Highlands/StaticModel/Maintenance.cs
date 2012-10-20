using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.StaticModel
{
    static public class Maintenance
    {
        #region Public Methods

        static IList<string> _letterGrades;
        static public IList<string> LetterGrades
        {
            get
            {

                if (_letterGrades == null)
                {
                    _letterGrades = ReadArrayFromFile("letterGrades");
                    if (_letterGrades == null)
                    {
                        _letterGrades = new List<string>() { "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D", "E", "-", "S", "N" };
                        WriteArrayToFile("letterGrades", _letterGrades);
                    }
                }
                return _letterGrades;
            }
        }


        static public List<string> GradeOrdinalLongs
        {
            get
            {
                return new List<string>() { "Kindergarten", "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth" };
            }
        }

        static public List<string> GradeLevelShorts
        {
            get
            {
                return new List<string>() { "K", "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th" };
            }
        }

        static IList<string> _subjects;
        static public IList<string> Subjects
        {
            get
            {

                if (_subjects == null)
                {
                    _subjects = ReadArrayFromFile("subjects");
                    if (_subjects == null)
                    {
                        _subjects = new List<string>() { "Reading", "Phonics/Spelling", "Written Expression", "Math", "Science", "Social Studies", "Mind That's Mine", "Art", "Music", "Health and Fitness" };
                        WriteArrayToFile("subjects", _subjects);
                    }
                }
                return _subjects;
            }
        }

        static public int GetSubjectIndex(string subject)
        {
            return Subjects.IndexOf(subject);
        }

        static IList<string> _selfDevelopmentAreas;
        static public IList<string> SelfDevelopmentAreas
        {
            get
            {

                if (_selfDevelopmentAreas == null)
                {
                    _selfDevelopmentAreas = ReadArrayFromFile("selfDevelopmentAreas");
                    if (_selfDevelopmentAreas == null)
                    {
                        _selfDevelopmentAreas = new List<string>() { "Focuses On Activity", "Shows conscientious effort", "Applies academic rules and strategies", "Completes class assignments accurately", "Completes homework accurately", "Demonstrates binder organization skills", "Demonstrates critical/creative thinking", "Demonstrates responsibility for self, supplies & equipment", "Demonstrates expected behavior", "Works cooperatively in groups", "Demonstrates clear verbal communication" };
                        WriteArrayToFile("selfDevelopmentAreas", _selfDevelopmentAreas);
                    }
                }
                return _selfDevelopmentAreas;
            }
        }

        static IList<string> _comments;
        static public IList<string> Comments
        {
            get
            {

                if (_comments == null)
                {
                    _comments = ReadArrayFromFile("comments");
                    if (_comments == null)
                    {
                        var rv = new List<string>();
                        rv.Add("(free-form)");
                        rv.Add("(General)");
                        rv.AddRange(_commentsGeneral);
                        rv.Add("(Written Expression)");
                        rv.AddRange(_commentsWrittenExpression);
                        rv.Add("(Non-Writer)");
                        rv.AddRange(_commentsNonWriter);
                        rv.Add(@"(Large Group/Content Area)");
                        rv.AddRange(_commentsLargeGroupContent);
                        rv.Add("(Needs Improvement)");
                        rv.AddRange(_commentsNeedsImprovement);
                        WriteArrayToFile("comments", rv);
                        _comments = rv;
                    }
                }
                return _comments;
            }
        }

        public static string FormatCommentFromList(string comment)
        {
            var rv = comment;
            if (rv.EndsWith("..."))
                rv = rv.Substring(0, rv.Length - 3);
            if (rv.EndsWith("…"))
                rv = rv.Substring(0, rv.Length - 1);
            if (rv.Contains("("))
            {
                var start = rv.IndexOf("(");
                rv = rv.Substring(0, start);
            }
            return rv.Trim();
        }

        static List<string> _commentsGeneral = new List<string>() { "A conscientious student with a good attitude toward learning", "Actively participates", "Applies phonetic skills", "Applying test taking strategies", "Applying what is being taught", "Appropriate work habits", "Asks for help when needed", "Becoming more consistent", "Beginning to apply…", "Beginning to develop independent writing skills", "Beginning to develop research skills for writing", "Beginning to develop research skills for writing a multi-paragraph paper", "Beginning to learn writing strategies", "Beginning to apply test taking strategies", "Consistently producing more accurate work", "Continues to make good progress(do not use on Updates)", "Continues to make progress(do not use on Updates)", "Making good progress (do not use on Updates)", "Continues to develop independent writing skills", "Demonstrates creativity", "Demonstrates knowledge of concepts", "Developing independent writing skills", "Developing skills of…", "Diligent worker", "Does well with…", "Doing well with application of decoding skills", "Enthusiastic about applying new writing strategies", "Enthusiastic learner", "Good at…", "Good retention of concepts", "Focused and attentive toward…", "Good application of concepts", "Good attitude", "Good background knowledge", "Good class participation", "Good effort", "Good effort to improve skills", "Good knowledge of…", "Good tests scores", "Grasps most ideas readily", "Growing in understanding", "Hard worker", "Has shown good improvement", "Has shown growth in understanding…", "Has shown progress learning vowel patterns", "Has shown steady growth", "Improved application of skills", "Is gaining self-confidence", "Is learning to follow directions more carefully", "Is working hard", "Keep up the good work", "Learning to apply strategies in daily work", "Making good progress (do not use on updates)", "Participates enthusiastically", "Participates with interest", "Pleasure to have in class (do not use ‘name’ is a pleasure to teach)", "Positive attitude; good attitude", "Puts forth good effort ", "Reading fluency improving", "Seems eager to improve", "Seems eager to learn", "Seems to enjoy learning new concepts", "Shows a positive attitude toward learning", "Shows enthusiasm in…", "Shows interest", "Steady improvement", "Strives to do good work", "Strives to improve writing skills", "Strives to learn new skills", "Striving to be independent", "Striving to improve skills", "Striving to learn…", "Strong effort in…", "Successfully applies concepts", "Tries to do well on…", "Test taking strategies ", "Understands and applies concepts", "Understands and successfully applies most concepts", "Understands concepts", "Understands concepts better than tests indicate", "Very conscientious", "Very cooperative", "Working to improve…", "Working to apply…strategies", "Works hard daily", "Works well with others" };
        static List<string> _commentsWrittenExpression = new List<string>() { "Developing independent writing skills", "Beginning to develop independent writing skills", "Developing basic sentence structure", "Developing paragraph writing skills", "Striving to improve writing skills", "Continues to develop writing skills", "Puts forth good effort to edit and add details to writing assignments", "Developing editing skills" };
        static List<string> _commentsNonWriter = new List<string>() { "Developing basic sentence structure (students should at least be working to write spelling sentences)" };
        static List<string> _commentsLargeGroupContent = new List<string>() { "Strived to learn facts about …", "Strived to learn about …", "Explored …", "Participated with interest while learning …", "Put forth good effort to learn …", "Displayed a positive attitude …", "Enthusiastically learned about …", "Quickly gained knowledge about …", "Actively participated …", "Developing knowledge about …" };
        static List<string> _commentsNeedsImprovement = new List<string>() { "Encouraging application of phonics rules in writing", "Encouraging application of phonics rules in reading", "Encouraging application of phonics rules in writing and reading", "Encouraging application of strategies when working independently", "Encouraging application of decoding skills", "Encourage to focus on details for greater accuracy", "Working on consistently applying decoding skills", "Working on consistently applying math strategies", "Working on developing skills of…", "Working on improving descriptive details", "Working on learning facts and applying strategies", "Working on ways to apply focusing strategies", "Working on developing study skill strategies", "Working to improve vocabulary skills", "Good test scores; inconsistent application", "Following through with homework will positively affect progress", "Needs guidance with revision concepts", "Reluctant to begin assignments", "Reluctant to extend his/her ideas", "Slowing down will improve accuracy", "Checking work will improve accuracy" };


        #endregion

        #region Private Methods

        internal static void WriteArrayToFile(string table, IList<string> array)
        {
            System.IO.File.WriteAllLines(table + ".txt", array);
        }

        internal static IList<string> ReadArrayFromFile(string table)
        {

            try
            {
                var rv = System.IO.File.ReadAllLines(table + ".txt");
                if (rv.Count() == 0)
                    return null;
                return rv;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static void WriteToFile(string key, string value)
        {
            System.IO.File.WriteAllText(key + ".txt", value);
        }

        internal static string ReadFromFile(string key)
        {
            try
            {
                return System.IO.File.ReadAllText(key + ".txt");
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        static IList<string> _groups;
        public static IList<string> Groups
        {
            get
            {

                if (_groups == null)
                {
                    _groups = ReadArrayFromFile("groups");
                    if (_groups == null)
                    {
                        _subjects = new List<string>() { "AM", "PM" };
                        WriteArrayToFile("groups", _subjects);
                    }
                }
                return _groups;
            }
        }
        static public IList<User> Teachers
        {
            get
            {
                return Users.Where(u => u.IsTeacher).ToList();
            }
        }

        static IList<User> _users;
        static public IList<User> Users
        {
            get
            {

                if (_users == null)
                {
                    _users = User.ParseUserStrings(ReadArrayFromFile("users"));
                    if (_users == null)
                    {
                        var rv  =
                            new List<string> { "I. Jones", "R. Jacobi", "I. Jasper", "S. Jackson", "M. Jeffers", "R. Jewell", "S. Jordan", "M. Judd", "R. Juliet", "S. Jakes", "I.  Kaiser", "R. Kaufman", "I. Kavanaugh", "S. Kaynard", "M. Keach", "R. Kellog", "S. Kelsey", "M. Kelso", "R. Kendall", "S. Kimble", "S. Lackey", "S. Lacy", "S. Ladd", "S. Lahey", "S. Laing", "S. Laird", "S. Lake", "S. Lalor", "S. Lam", "S. Lamb", "S. Lambert", "S. Lambourne", "S. Lamma", "S. Lamport", "S. Lancaster", "S. Meese", "S. Mouse", "S. Nightingale", "T.  O'Rourke" };
                        _users = new List<User>();
                        foreach (var name in rv)
                        {
                            var parts = name.Split(" ".ToCharArray());
                            RoleEnum role = RoleEnum.None;
                            if (parts[1].StartsWith("J"))
                                role = RoleEnum.ClassroomInstructor;
                            else if (parts[1].StartsWith("K"))
                                role = RoleEnum.SmallGroupInstructor;
                            else if (parts[1].StartsWith("L"))
                                role = RoleEnum.SpecialInstructor;
                            else if (parts[1].StartsWith("M"))
                                role = RoleEnum.Admin;
                            else if (parts[1].StartsWith("N"))
                                role = RoleEnum.Nurse;
                            else if (parts[1].StartsWith("O"))
                                role = RoleEnum.SuperUser;
                            _users.Add(new User(name, role, parts[1] + "@highlands.com" , null));
                        }
                        
                        foreach (RoleEnum role in Enum.GetValues(typeof(RoleEnum)))
                            _users.Add(new User(role.ToString(), role, role.ToString() + "@highlands.com", null));
                        _users.Add(new User("password", RoleEnum.SuperUser, "password@highlands.com", User.Hash("password", "password")));
                        
                        WriteArrayToFile("users", User.ComposeUserStrings(_users));
                    }
                }
                return _users;
            }
        }
        static public void SaveUsers()
        {
            WriteArrayToFile("users", User.ComposeUserStrings(_users));
        }


        internal static string GradeLevelNumber(string gradeLevel)
        {
            if (gradeLevel == null)
                return null;
            if (gradeLevel == "K")
                return "0";
            return gradeLevel.Substring(0, 1);
        }
    }

    public enum ApprovalStage
    {
        None,
        Open,
        Instructor,
        Classroom,
        Office
    }
}
