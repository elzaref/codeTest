using System.Diagnostics;
using System.Linq;
using Frontend.Utilities;
using DataComponent.Repositories.Interfaces;
using DomainModels.Models;
using DomainModels.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Frontend.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IUserRepository _userRepository;

		public HomeController(ILogger<HomeController> logger, IUserRepository userRepository)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		}

		public IActionResult Index()
		{
			var UsersViewModel = GetUsersViewModel();
			return View("Users", UsersViewModel);
		}
		public IActionResult CreateUser(User user)
		{
			//if(user.Id)
			_userRepository.AddSync(user);
			//var UsersViewModel = GetUsersViewModel();
			return RedirectToAction("Index");
			//return View("Users", UsersViewModel);
		}
		public IActionResult ReadUser(string id)
		{
			var User = _userRepository.GetSingleByExpressionSync(u => u.Id == id);
			if(User==null)
			{
				return RedirectToAction("Index");
			}
			return View(User);
		}
		public IActionResult UpdateUser(User user)
		{
			_userRepository.ReplaceOneSync(user.Id, user);
			return RedirectToAction("Index"); 
		}

		public IActionResult Delete(string id)
		{
			_userRepository.DeleteSync(x => x.Id == id);
			return RedirectToAction("Index");
		}
		public IActionResult CreateForm()
		{
			return View(new User());
		}
		public IActionResult UpdateForm(string id)
		{
			var User = _userRepository.GetSingleByExpressionSync(u => u.Id == id);
			if (User == null)
			{
				var UsersViewModel = GetUsersViewModel();
				return View("Users", UsersViewModel);
			}
			return View(User);
		}

		[NonAction]
		public UsersViewModel GetUsersViewModel()
		{
			var users = _userRepository.GetAllSync();

			if (users.Count > 0)
			{
				return new UsersViewModel { Users = users.ToList() };
			}

			var usersFromJson = new GetUsersFromJson("users.json").Execute();
			_userRepository.AddManySync(usersFromJson);
			users = usersFromJson.ToList();
			return new UsersViewModel { Users = users.ToList() };
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel
			{
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			});
		}
	}
}