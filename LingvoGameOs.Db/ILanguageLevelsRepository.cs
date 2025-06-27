using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface ILanguageLevelsRepository
    {
        Task<LanguageLevel?> GetExistingLanguageLevelAsync(string nameSelectedLangLvl);
    }
}