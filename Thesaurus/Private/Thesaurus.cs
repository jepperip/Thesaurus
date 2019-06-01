using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Fluent;

namespace Thesaurus.Private
{
	/// <summary>
	/// A simple thesaurus
	/// </summary>
	internal class Thesaurus : IThesaurus
	{
		private readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly object dictionaryLock = new object();

		private readonly Dictionary<string, ISet<string>> synonymDictionary = new Dictionary<string, ISet<string>>();

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
		public void AddSynonyms(IEnumerable<string> synonyms)
		{
			log.Debug("Starting adding synonyms...");

			if (synonyms == null)
			{
				log.Error("Failed to add synonyms: input was null");
				throw new ArgumentNullException(nameof(synonyms), "Value cannot be null");
			}

			if (synonyms.Any(word => word == null))
			{
				log.Error("Failed to add synonyms: input contained null values");
				throw new ArgumentException("Input values cannot be null");
			}

			if (synonyms.Count() < 2)
			{
				log.Error("Failed to add synonyms: input count was less than 2");
				throw new ArgumentException("List if synonyms must contain two or more elements");
			}

			AddSynonymsInternal(synonyms.ToList());
		}
		
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
		public IEnumerable<string> GetSynonyms(string word)
		{
			if(word == null)
			{
				log.Error("Failed to get synonyms: input was null");
				throw new ArgumentNullException(nameof(word), "Value cannot be null");
			}

			// This lock will ensure that any synonyms that are in the process of being added
			// gets added before we return the result.
			lock (dictionaryLock)
			{
				if (synonymDictionary.ContainsKey(word))
				{
					return synonymDictionary[word];
				}

				log.Warn(
					"Requested synonyms for word that is not present in the thesaurus; '{0}'", word);
				return Enumerable.Empty<string>();
			}
		}

		/// <summary>
		/// Gets all words that are stored in the thesaurus
		/// </summary>
		public IEnumerable<string> GetWords()
		{
			// We need a lock here otherwise there is a small risk of including words
			// that do not yet have had their synonyms added.
			lock (dictionaryLock)
			{
				return synonymDictionary.Keys;
			}
		}

		private void AddSynonymsInternal(IReadOnlyList<string> synonyms)
		{
			foreach (string word in synonyms)
			{
				// The word is not considered a synonym to itself, let's remove it.
				// Note: it would be faster to remove the word from the has set afterwards, especially for long lists of string.
				// I have chosen not to do that because it would mean that we would have to remove it inside the lock to avoid unexpected behaviour,
				// which makes the code somewhat more fragile.
				var synonymsToAdd = synonyms.Except(new[] { word });

				// This lock prevents a race condition where adding the same word twice could overwrite existing synonymous
				lock (dictionaryLock)
				{
					// Note: there are some handy dictionary-method missing in .NET standard 2.0, like TryAdd,
					// that could be used here instead.
					if (!synonymDictionary.ContainsKey(word))
					{
						synonymDictionary[word] = new HashSet<string>();
					}
					
					// Add the synonyms
					synonymDictionary[word].UnionWith(synonymsToAdd);
				}

				log.Debug("Added or updated {0} synonyms to '{0}'", synonymsToAdd.Count(), word);
			}

			log.Debug("Finished adding synonyms");
		}
	}
}