using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Program
{
	public static void Main()
	{
		var teachers = new List<Teacher>(); // Заполнить из файла Учетиля.txt
		var students = new List<Student>(); // Заполнить из файла Ученики.txt
		var exams = new List<Exams>();

		using (StreamReader sr = new StreamReader("Ученики.txt"))
		{
			string line;
			bool first = true;
			int indID = 0;
			while ((line = sr.ReadLine()) != null)
			{
				string[] parts = line.Split("\t");

				if (!first)
				{
					parts = parts.Where(p => !string.IsNullOrEmpty(p)).ToArray();

					var student = new Student
					{
						ID = indID,
						Name = parts[0],
						LastName = parts[1],
						Age = int.Parse(parts[2])
					};
					students.Add(student);
					indID++;
				}
				first = false;
            }

			//for (int i = 0; i < students.Count; i++)
   //         {
   //             Console.WriteLine(students[i].ID);
   //             Console.WriteLine(students[i].Name);
   //             Console.WriteLine(students[i].LastName);
   //             Console.WriteLine(students[i].Age);
   //             Console.WriteLine("---------------------------------------");
			//}
		}

		using (StreamReader sr = new StreamReader("Учетиля.txt"))
		{
			string line;
			bool first = true;
			int indID = 0;
			while ((line = sr.ReadLine()) != null)
			{
				string[] parts = line.Split("\t");

				if (!first)
				{
					parts = parts.Where(p => !string.IsNullOrEmpty(p) && (char.IsLetter(p[0]) || char.IsNumber(p[0]))).ToArray();
					LessonType lType = new LessonType();
					switch (parts[3])
                    {
						case "Математика":
							lType = LessonType.Mathematics;
							break;
						case "Физика":
							lType = LessonType.Physics;
							break;

                    }
					var teacher = new Teacher
					{
						ID = indID,
						Name = parts[0],
						LastName = parts[1],
						Age = int.Parse(parts[2]),
						Lesson = lType
					};
					teachers.Add(teacher);
					indID++;
				}
				first = false;
			}

            //for (int i = 0; i < teachers.Count; i++)
            //{
            //    Console.WriteLine(teachers[i].ID);
            //    Console.WriteLine(teachers[i].Name);
            //    Console.WriteLine(teachers[i].LastName);
            //    Console.WriteLine(teachers[i].Age);
            //    Console.WriteLine(teachers[i].Lesson);
            //    Console.WriteLine("---------------------------------------");
            //}
        }

		Random random = new Random();

		//1. Найти учителя у которого в классе меньше всего учеников 

		Console.WriteLine("---------------- Вопрос - 1 ------------------");

		for (int i = 0; i < 1000; i++)
        {
			var randTeacher = teachers[random.Next(teachers.Count)];
			var randStudent = students[random.Next(students.Count)];
			var randYear = 2020 + random.Next(3);
			int year = random.Next(2020, 2024);
			int month = random.Next(1, 13);
			int day = random.Next(1, DateTime.DaysInMonth(year, month));


			var exam = new Exams
			{
				Lesson = randTeacher.Lesson,
				StudentId = randStudent.ID,
				TeacherId = randTeacher.ID,
				Score = random.Next(0, 101),
				ExamDate = new DateTime(year, month, day),
				Student = randStudent,
				Teacher = randTeacher
			};
			exams.Add(exam);
		}

		var teachersExams = teachers
			.GroupJoin(exams, teacher => teacher.ID, exam => exam.TeacherId, (teacher, exams) => new
			{
				Teacher = teacher,
				StudentCount = exams.Count()
			})
			.ToArray();


		
		Console.WriteLine("Учителя и количество их экзаменов:");
		for (int i = 0; i < teachersExams.Count(); i++)
        {
            Console.WriteLine(teachersExams[i].Teacher.Name);
			Console.WriteLine(teachersExams[i].Teacher.LastName);
			Console.WriteLine(teachersExams[i].StudentCount);
			Console.WriteLine("-------------------------------------------");
		}

		var teacherWithFewestStudents = teachers
			.GroupJoin(exams, teacher => teacher.ID, exam => exam.TeacherId, (teacher, exams) => new
			{
				Teacher = teacher,
				StudentCount = exams.Count()
			})
			.OrderByDescending(group => group.StudentCount)
			.First();

		Console.WriteLine("Учитель с наибольшим количеством учеников:");
		Console.WriteLine(teacherWithFewestStudents.Teacher.Name);
		Console.WriteLine(teacherWithFewestStudents.Teacher.LastName);
		Console.WriteLine(teacherWithFewestStudents.StudentCount);


		//2. Найти средний бал экзамена по Физики за 2023 год.	
		Console.WriteLine("---------------- Вопрос - 2 ------------------");

		var averageScore = exams
			.Where(exam => exam.Lesson == LessonType.Physics && exam.ExamDate.Year == 2023)
			.Average(exam => exam.Score);

		Console.WriteLine("Средний балл экзаменов по физике за 2023 год: ");
		Console.WriteLine(averageScore);


		//3. Получить количество учиников которые по экзамену Математики получили больше 90 баллов, где учитель Alex 

		Console.WriteLine("---------------- Вопрос - 3 ------------------");

		var count = exams
			.Where(exam => exam.Lesson == LessonType.Mathematics && exam.Score > 90 && exam.Teacher.Name == "Alex")
			.Count();

		Console.WriteLine("Количество учеников, которые по экзамену математики получили больше 90 баллов, где учитель Alex:");
		Console.WriteLine(count);

	}

	public class Person
	{
		public long ID { get; set; }
		public string Name { get; set; }
		public string LastName { get; set; }
		public int Age { get; set; }
	}

	public class Teacher : Person
	{
		public LessonType Lesson { get; set; }
	}

	public class Student : Person
	{

	}

	public class Exams
	{
		public LessonType Lesson { get; set; }

		public long StudentId { get; set; }
		public long TeacherId { get; set; }

		public decimal Score { get; set; }
		public DateTime ExamDate { get; set; }

		public Student Student { get; set; }
		public Teacher Teacher { get; set; }
	}

	public enum LessonType
	{
		Mathematics = 1,
		Physics = 2
	}

	
}