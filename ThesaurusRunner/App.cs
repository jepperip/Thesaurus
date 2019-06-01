using System;
using System.Linq;
using Thesaurus;

namespace ThesaurusRunner
{
	internal class App
	{
		private readonly IThesaurus thesaurus;

		public App(IThesaurus thesaurus)
		{
			this.thesaurus = thesaurus;
		}

		public void Run()
		{
			bool run = true;
			while (run)
			{
				Console.Clear();
				Console.WriteLine(
					"Choose an option:\n" +
					"1. Add synonyms to the thesaurus\n" +
					"2. List synonyms for a word\n" +
					"3. List all added words\n" +
					"4. Exit");

				var t = Console.ReadKey(true);

				switch (t.Key)
				{
					case ConsoleKey.Escape:
					case ConsoleKey.D4:
						run = false;
						break;
					case ConsoleKey.D1:
						Console.Clear();
						AddSynonyms();
						break;
					case ConsoleKey.D2:
						Console.Clear();
						ListSynonymsForWord();
						break;
					case ConsoleKey.D3:
						Console.Clear();
						ListAllWords();
						break;
				}
			}
		}

		private void ListAllWords()
		{
			var words = thesaurus.GetWords();

			if (words.Any())
			{
				foreach (string word in words)
				{
					Console.WriteLine(word);
				}
			}
			else
			{
				Console.WriteLine("The thesaurus does not contain any words! Try adding some");
			}

			Console.ReadKey();
		}

		private void ListSynonymsForWord()
		{
			Console.WriteLine("Input the word you want the synonyms of:");
			string word = Console.ReadLine();

			Console.Clear();
			var synonyms = thesaurus.GetSynonyms(word);

			if (synonyms.Any())
			{
				Console.WriteLine("Synonyms for '{0}' are:", word);
				foreach (string synonym in synonyms)
				{
					Console.WriteLine(synonym);
				}
			}
			else
			{
				Console.WriteLine("The word '{0}' doesn't have any synonyms", word);
			}
			
			Console.ReadKey();
		}

		private void AddSynonyms()
		{
			Console.WriteLine("Input synonyms in a comma-separated list (ex 'cat,feline,kitty'):");

			var synonyms = GetCommaSeperatedListFromUser();

			if (synonyms.Length < 2)
			{
				Console.WriteLine("Input 2 or more words");
				AddSynonyms();
				return;
			}

			thesaurus.AddSynonyms(synonyms);
		}

		private string[] GetCommaSeperatedListFromUser()
		{
			string input = Console.ReadLine().Replace(" ", "");
			return input.Split(',');
		}
	}
}
