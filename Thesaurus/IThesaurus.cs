using System;
using System.Collections.Generic;

namespace Thesaurus
{
	/// <summary>
	/// A simple thesaurus
	/// </summary>
	/// <remarks>This library uses NLog for logging</remarks>
	public interface IThesaurus
	{
		/// <summary>
		/// Adds the give words as synonyms to each other
		/// </summary>
		/// <param name="synonyms">A list of words that are considered synonyms to each other</param>
		/// <exception cref="ArgumentException">
		/// Input value contains null values or contains fewer than two items
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Input value is null
		/// </exception>
		/// <remarks>This method is thread-safe</remarks>
		void AddSynonyms(IEnumerable<string> synonyms);

		/// <summary>
		/// Gets the synonyms for a word
		/// </summary>
		/// <returns>
		/// A list of synonyms for the given word, or an empty list
		/// if the given word is not present in the thesaurus
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Input value is null
		/// </exception>
		IEnumerable<string> GetSynonyms(string word);

		/// <summary>
		/// Gets all words that are stored in the thesaurus
		/// </summary>
		IEnumerable<string> GetWords();
	}
}