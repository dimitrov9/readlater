using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadLater.Entities;
using ReadLater.Repository;

namespace ReadLater.Services
{
    public class CategoryService : ICategoryService
    {
        protected IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Category CreateCategory(Category category)
        {
            _unitOfWork.Repository<Category>().Insert(category);
            _unitOfWork.Save();
            return category;
        }

        public void UpdateCategory(Category category)
        {
            _unitOfWork.Repository<Category>().Update(category);
            _unitOfWork.Save();
        }

        public List<Category> GetCategories()
        {
            return _unitOfWork.Repository<Category>().Query().Get().ToList();
        }

        public Category GetCategory(int Id)
        {
            return _unitOfWork.Repository<Category>().Query()
                                                    .Filter(c => c.ID == Id)
                                                    .Get()
                                                    .FirstOrDefault();
        }

        public Category GetCategory(string Name)
        {
            return _unitOfWork.Repository<Category>().Query()
                                        .Filter(c => c.Name == Name)
                                        .Get()
                                        .FirstOrDefault();
        }

        public void DeleteCategory(Category category)
        {
            var bookmarks = _unitOfWork.Repository<Bookmark>().Query()
                .Filter(x => x.CategoryId == category.ID)
                .Get()
                .ToList();

            foreach (var bookmark in bookmarks)
            {
                bookmark.Category = null;
                bookmark.CategoryId = null;
                _unitOfWork.Repository<Bookmark>().Update(bookmark);
            }

            _unitOfWork.Repository<Category>().Delete(category);
            _unitOfWork.Save();
        }
    }
}
