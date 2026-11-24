
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class CategoryService
    {
        // Lấy toàn bộ danh mục.
        public static List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            XmlDocument document =
                XmlDataAccess.LoadXml("Categories.xml");

            XmlNodeList nodes =
                document.SelectNodes("/Categories/Category");

            foreach (XmlNode node in nodes)
            {
                categories.Add(MapCategory(node));
            }

            categories.Sort((a, b) =>
                a.DisplayOrder.CompareTo(b.DisplayOrder));

            return categories;
        }

        // Tìm danh mục theo CategoryId.
        public static Category GetCategoryById(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                return null;

            foreach (Category category in GetAllCategories())
            {
                if (string.Equals(
                    category.CategoryId,
                    categoryId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return category;
                }
            }

            return null;
        }

        // Tìm danh mục theo tên.
        public static Category GetCategoryByName(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return null;

            foreach (Category category in GetAllCategories())
            {
                if (string.Equals(
                    category.CategoryName,
                    categoryName.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return category;
                }
            }

            return null;
        }

        // Lấy danh sách danh mục theo trạng thái.
        public static List<Category> GetCategoriesByStatus(string status)
        {
            List<Category> result = new List<Category>();

            if (string.IsNullOrWhiteSpace(status))
                return result;

            foreach (Category category in GetAllCategories())
            {
                if (string.Equals(
                    category.Status,
                    status.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(category);
                }
            }

            return result;
        }

        // Lấy các danh mục đang hoạt động.
        public static List<Category> GetActiveCategories()
        {
            return GetCategoriesByStatus("Active");
        }

        // Đếm tổng số danh mục.
        public static int CountAllCategories()
        {
            return GetAllCategories().Count;
        }

        // Đếm danh mục theo trạng thái.
        public static int CountCategoriesByStatus(string status)
        {
            return GetCategoriesByStatus(status).Count;
        }

        // Chuyển dữ liệu XML thành đối tượng Category.
        private static Category MapCategory(XmlNode node)
        {
            return new Category
            {
                CategoryId = GetValue(node, "CategoryId"),
                CategoryName = GetValue(node, "CategoryName"),
                Description = GetValue(node, "Description"),
                Image = GetValue(node, "Image"),
                DisplayOrder = GetIntValue(node, "DisplayOrder"),
                Status = GetValue(node, "Status")
            };
        }

        // Đọc giá trị XML dạng chuỗi.
        private static string GetValue(
            XmlNode parent,
            string elementName)
        {
            XmlNode node = parent[elementName];

            return node == null
                ? string.Empty
                : node.InnerText.Trim();
        }

        // Đọc giá trị số nguyên.
        private static int GetIntValue(
            XmlNode parent,
            string elementName)
        {
            int value;

            return int.TryParse(
                GetValue(parent, elementName),
                out value) ? value : 0;
        }
    }
}
