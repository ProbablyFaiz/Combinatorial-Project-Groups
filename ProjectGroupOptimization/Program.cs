using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGroupOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Student> students = new List<Student>();
            using (var reader = new StreamReader(@"C:\Users\Faiz\OneDrive\Documents\Miscellaneous\CombinatorialProjectGroupData.csv"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var tokens = line.Split('\t');
                    var student = new Student
                    {
                        Name = Convert.ToString(tokens[0]),
                        BuildingSkill = Convert.ToInt32(tokens[1]),
                        WritingSkill = Convert.ToInt32(tokens[2]),
                        OrganizationSkill = Convert.ToInt32(tokens[3])
                    };
                    students.Add(student);
                }
            }
            Student[] studentArray = students.ToArray();
            Optimization optimizer = new Optimization(studentArray, 4);
            optimizer.Optimize();
        }
    }

    class Optimization
    {
        Team[] Teams { get; set; }
        double[] SkillMeans { get; set; }
        int Attempts { get; set; }

        public Optimization(Student[] students, int groupSize)
        {
            GetSkillMeans(students);
            AssignInitialGroups(students, groupSize);
            Attempts = 0;
        }

        public void Optimize()
        {
            Console.WriteLine("Current Penalty:" + GetGlobalCost());

            foreach (Team team in Teams)
                Console.WriteLine(team.ToString());
            for (int i = 0; i < 50000; i++)
            {
                if (!TrySwap())
                    TryThreeWaySwap();
                //Console.WriteLine(GetGlobalCost());
            }
            Console.WriteLine("New Teams: \n");
            foreach (Team team in Teams)
                Console.WriteLine(team.ToString());
            Console.WriteLine("New Penalty:" + GetGlobalCost());
        }

        public double GetLocalCost(Team team)
        {
            double[] summations = GetSkillSummations(team);
            return GetSkillCost(SkillMeans[0] * team.Members.Count, summations[0]) + GetSkillCost(SkillMeans[1] * team.Members.Count, summations[1]) + GetSkillCost(SkillMeans[2] * team.Members.Count, summations[2]);
        }

        public double[] GetSkillSummations(Team team)
        {
            double[] sums = new double[3];
            foreach (Student student in team.Members)
            {
                sums[0] += student.BuildingSkill;
                sums[1] += student.WritingSkill;
                sums[2] += student.OrganizationSkill;
            }
            return sums;
        }

        public double[] GetSkillAverages(Team team)
        {
            double[] sums = new double[3];
            foreach (Student student in team.Members)
            {
                sums[0] += student.BuildingSkill;
                sums[1] += student.WritingSkill;
                sums[2] += student.OrganizationSkill;
            }
            double[] averages = new double[3];
            for (int i = 0; i < sums.Length; i++)
                averages[i] = sums[i] / team.Members.Count;
            return averages;
        }

        public double[] GetMeanOfSkillMeans(double[][] teamSkillMeans)
        {
            double[] sumsOfMeans = new 
            for (int i = 0; i < 5; i++)
            {

            }

            double average = teamSkillMeans[0].Average();
            double sumOfSquaresOfDifferences = teamSkillMeans[0].Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / someDoubles.Length);
        }

        public double GetGlobalCost()
        {
            double costSum = 0;
            foreach (Team team in Teams)
                costSum += GetLocalCost(team);
            return costSum;
        }

        public double GetSkillCost(double skillMean, double teamSkill)
        {   //Penalization becomes progressively more severe further below the mean
            return skillMean - teamSkill > 0 ? Math.Pow(skillMean - teamSkill, 2) : 0;
        }

        public bool TrySwap()
        {   //Attempt two way swap
            Random random = new Random();
            Team firstTeam = Teams[random.Next(Teams.Length)];
            Team secondTeam = Teams[random.Next(Teams.Length)];
            double currentLocalPenaltyScore = GetLocalCost(firstTeam) + GetLocalCost(secondTeam);

            if (firstTeam == secondTeam)
                return false;
            Student firstTeamMember = firstTeam.Members[random.Next(firstTeam.Members.Count)];
            Student secondTeamMember = secondTeam.Members[random.Next(secondTeam.Members.Count)];

            firstTeam.Members.Remove(firstTeamMember);
            firstTeam.Members.Add(secondTeamMember);
            secondTeam.Members.Remove(secondTeamMember);
            secondTeam.Members.Add(firstTeamMember);

            double newLocalPenaltyScore = GetLocalCost(firstTeam) + GetLocalCost(secondTeam);
            if (newLocalPenaltyScore > currentLocalPenaltyScore)
            {
                firstTeam.Members.Remove(secondTeamMember);
                firstTeam.Members.Add(firstTeamMember);
                secondTeam.Members.Remove(firstTeamMember);
                secondTeam.Members.Add(secondTeamMember);
                return false;
            }
            return true;
        }

        public bool TryThreeWaySwap()
        {   //Attempt two way swap
            Random random = new Random();
            Team firstTeam = Teams[random.Next(Teams.Length)];
            Team secondTeam = Teams[random.Next(Teams.Length)];
            Team thirdTeam = Teams[random.Next(Teams.Length)];
            double currentLocalPenaltyScore = GetLocalCost(firstTeam) + GetLocalCost(secondTeam) + GetLocalCost(thirdTeam);

            if (firstTeam == secondTeam || firstTeam == thirdTeam || secondTeam == thirdTeam)
               return false;
            Student firstTeamMember = firstTeam.Members[random.Next(firstTeam.Members.Count)];
            Student secondTeamMember = secondTeam.Members[random.Next(secondTeam.Members.Count)];
            Student thirdTeamMember = thirdTeam.Members[random.Next(thirdTeam.Members.Count)];

            firstTeam.Members.Remove(firstTeamMember);
            firstTeam.Members.Add(thirdTeamMember);
            secondTeam.Members.Remove(secondTeamMember);
            secondTeam.Members.Add(firstTeamMember);
            thirdTeam.Members.Remove(thirdTeamMember);
            thirdTeam.Members.Add(secondTeamMember);

            double newLocalPenaltyScore = GetLocalCost(firstTeam) + GetLocalCost(secondTeam);
            if (newLocalPenaltyScore > currentLocalPenaltyScore)
            {
                firstTeam.Members.Remove(thirdTeamMember);
                firstTeam.Members.Add(firstTeamMember);
                secondTeam.Members.Remove(firstTeamMember);
                secondTeam.Members.Add(secondTeamMember);
                thirdTeam.Members.Remove(secondTeamMember);
                thirdTeam.Members.Add(thirdTeamMember);
                return false;
            }
            return true;
        }

        public void GetSkillMeans(Student[] students)
        {
            int[] skillSums = new int[3];
            foreach (Student student in students)
            {
                skillSums[0] += student.BuildingSkill;
                skillSums[1] += student.WritingSkill;
                skillSums[2] += student.OrganizationSkill;
            }
            double[] skillMeans = new double[3];
            for (int i = 0; i < skillMeans.Length; i++)
                skillMeans[i] = (skillSums[i] + 0.0) / students.Length;
            SkillMeans = skillMeans;
        }

        public void AssignInitialGroups(Student[] students, int groupSize)
        {
            int numberOfGroups = students.Length / groupSize;
            Teams = new Team[numberOfGroups];
            for (int i = 0; i < Teams.Length; i++)
            {
                Teams[i] = new Team();
            }
            for (int i = 0; i < students.Length; i++)
            {
                Teams[i % numberOfGroups].Members.Add(students[i]);
            }
        }

    }

    class Student
    {
        public string Name { get; set; }
        public int BuildingSkill { get; set; }
        public int WritingSkill { get; set; }
        public int OrganizationSkill { get; set; }
        public List<Student> PreferredPartners { get; set; }

        public override string ToString()
        {
            return "Name: " + Name + ", Building Skill: " + BuildingSkill.ToString() + ", Writing Skill: " + WritingSkill.ToString() + ", Organization: " + OrganizationSkill.ToString();
        }
    }

    class Team
    {
        public List<Student> Members { get; set; }
        public double CurrentPenaltyScore { get; set; }
        public double[] SkillSummations { get; set; }

        public Team()
        {
            Members = new List<Student>();
        }

        public override string ToString()
        {
            string teamString = "";
            for (int i = 0; i < Members.Count; i++)
            {
                teamString += Members[i].ToString() + "\n";
            }
            return teamString;
        }
    }
}