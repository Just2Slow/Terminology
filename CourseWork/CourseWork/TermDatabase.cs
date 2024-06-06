using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CourseWork
{
    public class TermDatabase
    {
        private List<Term> terms;

        public TermDatabase()
        {
            terms = new List<Term>();
        }

        public void AddTerm(Term term)
        {
            terms.Add(term);
            terms = terms.OrderBy(t => t.Name).ToList();
        }

        public void DeleteTerm(string termName)
        {
            var termToDelete = terms.FirstOrDefault(t => t.Name == termName);
            if (termToDelete != null)
            {
                terms.Remove(termToDelete);
                foreach (var term in terms)
                {
                    term.References.Remove(termName);
                }
            }
        }

        public List<Term> SearchTerms(string query)
        {
            query = query.ToLower();
            return terms.Where(t => t.Name.ToLower().Contains(query) ||
                                    t.Definition.ToLower().Contains(query)).ToList();
        }

        public void UpdateReferences(string oldName, string newName)
        {
            foreach (var term in terms)
            {
                if (term.References.Contains(oldName))
                {
                    term.References.Remove(oldName);
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        term.References.Add(newName);
                    }
                }
            }
        }

        public List<Term> GetAllTerms()
        {
            return terms;
        }

        public void SaveToFile(string filePath)
        {
            var json = JsonConvert.SerializeObject(terms, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                terms = JsonConvert.DeserializeObject<List<Term>>(json) ?? new List<Term>();
            }
        }
    }
}
