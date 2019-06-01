using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Thesaurus.Test
{
	[TestFixture(TestOf = typeof(IThesaurus))]
	public class ThesaurusTest
	{
		[TestCase(TestName = "Given simple valid input - should return expected number of results")]
		public void ValidInput_ReturnsExpectedResults()
		{
			var subject = new Private.Thesaurus();
			var input = new[] {"a", "b", "c", "d",};

			subject.AddSynonyms(input);

			var result = subject.GetWords();

			Assert.AreEqual(
				input.Length,
				result.Count(),
				"Length of the input should be equal to length of the result");
		}

		[TestCase(TestName = "Given single list of synonyms - should return the expected synonyms for every word")]
		public void GivenSingleListOfSynonyms_ShouldReturnExpectedSynonyms()
		{
			var subject = new Private.Thesaurus();
			var input = new[] {"a", "b", "c", "d"};

			subject.AddSynonyms(input);

			foreach (string word in input)
			{
				var otherWords = input.Except(new[] {word});
				var synonyms = subject.GetSynonyms(word);
				Assert.IsTrue(
					otherWords.All(synonyms.Contains),
					"The list of synonyms should contain all the other given words");
			}
		}

		[TestCase(TestName =
			"Given recurring word - should consider the recurring word to be synonymous with all other inputted words")]
		public void MultipleOccurrences_ShouldReturnAllOccurrences()
		{
			var subject = new Private.Thesaurus();

			const string recurringWord = "a";

			var input1 = new[] {recurringWord, "b"};
			var input2 = new[] {recurringWord, "c"};
			var input3 = new[] {recurringWord, "d"};

			subject.AddSynonyms(input1);
			subject.AddSynonyms(input2);
			subject.AddSynonyms(input3);

			var recurringWordSynonyms = subject.GetSynonyms(recurringWord);

			Assert.IsTrue(recurringWordSynonyms.Contains("b"));
			Assert.IsTrue(recurringWordSynonyms.Contains("c"));
			Assert.IsTrue(recurringWordSynonyms.Contains("d"));
		}

		[TestCase(TestName = "Given a large list of synonyms - should be able to return expected result")]
		public void GivenLargeListOfSynonyms_ShouldReturnExpectedResult()
		{
			var subject = new Private.Thesaurus();

			var input = CreateRandomUniqueWords(5000);

			subject.AddSynonyms(input);

			string first = input.First();
			var synonyms = subject.GetSynonyms(first);

			Assert.IsTrue(synonyms.All(s => input.Contains(s)));
		}

		[TestCase(TestName = "Given large amounts of synonym lists - should be able to return expected result")]
		public void GivenLargeOfAmountOfSynonymLists_ShouldReturnExpectedResult()
		{
			var subject = new Private.Thesaurus();
			const int numberOfSynonymLists = 10000;
			const int numberOfSynonymsPerList = 5;
			const string controlWord = "x";

			for (int i = 0; i < numberOfSynonymLists; i++)
			{
				var input = CreateRandomUniqueWords(numberOfSynonymsPerList).ToList();

				// Add a control word
				input.Add(controlWord);

				subject.AddSynonyms(input);
			}

			// Test that the control word given to all the random inputs are present
			// Since the control word is synonymous with all the random words,
			// the number of synonymous given should be equal to the total number of generated random word.
			var synonyms = subject.GetSynonyms(controlWord);
			int expectedNumberOfSynonyms = numberOfSynonymLists * numberOfSynonymsPerList;
			Assert.AreEqual(expectedNumberOfSynonyms, synonyms.Count());
		}

		[TestCase(TestName = "Given duplicate words as input - should not add duplicates")]
		public void GivenDuplicateWords_ShouldNotBeAdded()
		{
			var subject = new Private.Thesaurus();
			var input = new[] {"a", "b"};

			subject.AddSynonyms(input);
			subject.AddSynonyms(input);

			var input2 = new[] {"a", "c", "d"};
			subject.AddSynonyms(input2);
			var result = subject.GetWords();

			Assert.AreEqual(4, result.Count());
		}

		[TestCase(TestName =
			"Given several inputs - should return the expected number of elements when getting all words")]
		public void GivenSeveralInputs_ShouldReturnCorrectNumberOfTotalWords()
		{
			var subject = new Private.Thesaurus();
			var input1 = new[] {"a", "b", "c"};
			var input2 = new[] {"d", "e", "f"};

			subject.AddSynonyms(input1);
			subject.AddSynonyms(input2);

			var result = subject.GetWords();

			Assert.AreEqual(input1.Length + input2.Length, result.Count());
		}

		[TestCase(TestName = "Given invalid input - should throw the expected exception")]
		public void InputValidation()
		{
			var subject = new Private.Thesaurus();

			var inputWithNullValues = new[] {"a", "b", null};
			var inputWithLessThan2Values = new[] {"a"};

			Assert.Throws<ArgumentNullException>(() => subject.AddSynonyms(null));
			Assert.Throws<ArgumentException>(() => subject.AddSynonyms(inputWithNullValues));
			Assert.Throws<ArgumentException>(() => subject.AddSynonyms(inputWithLessThan2Values));
			Assert.Throws<ArgumentNullException>(() => subject.GetSynonyms(null));
		}

		private static IEnumerable<string> CreateRandomUniqueWords(int numberOfWords)
		{
			var rand = new Random();
			var result = new List<string>();
			for (var i = 0; i < numberOfWords; i++)
			{
				result.Add(i + "_" + rand.Next());
			}

			return result;
		}
	}
}