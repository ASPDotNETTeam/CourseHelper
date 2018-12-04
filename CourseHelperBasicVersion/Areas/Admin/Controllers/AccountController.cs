using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseHelperBasicVersion.Models;
using CourseHelperBasicVersion.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseHelperBasicVersion.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<CourseHelperUser> userManager;
        private SignInManager<CourseHelperUser> signInManager;
        private IStudentDatabase studentDB;
        private IUserValidator<CourseHelperUser> userValidator;
        private IPasswordValidator<CourseHelperUser> passwordValidator;
        private IPasswordHasher<CourseHelperUser> passwordHasher;

        public AccountController(UserManager<CourseHelperUser> userManager, SignInManager<CourseHelperUser> signInManager, IStudentDatabase studentDB, IUserValidator<CourseHelperUser> usrVal, IPasswordValidator<CourseHelperUser> pswdVal, IPasswordHasher<CourseHelperUser> pswdHash)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.studentDB = studentDB;
            this.userValidator = usrVal;
            this.passwordValidator = pswdVal;
            this.passwordHasher = pswdHash;
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View("Create", new UserModel());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateFaculty()
        {
            return View("Create", new UserModel() { Role = 1 });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.IsInRole("Admin") && userModel.Role == 1)
                {
                    CourseHelperUser facultyUser = new CourseHelperUser
                    {
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        UserName = userModel.UserName
                    };
                    IdentityResult facultyResult = await userManager.CreateAsync(facultyUser, userModel.Password);
                    if (facultyResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(facultyUser, "Faculty");
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        foreach (IdentityError error in facultyResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    //gets the largest(last) student number in database, and adds 1 (generates unique studentnumber)
                    long studentNumber = studentDB.Students.OrderBy(s => s.StudentNumber).Last().StudentNumber + 1;
                    CourseHelperUser studentUser = new CourseHelperUser
                    {
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        UserName = userModel.UserName,
                        StudentNumber = studentNumber
                    };

                    IdentityResult studentResult = await userManager.CreateAsync(studentUser, userModel.Password);
                    if (studentResult.Succeeded)
                    {
                        Student student = new Student()
                        {
                            StudentId = 0,
                            FirstName = userModel.FirstName,
                            LastName = userModel.LastName,
                            StudentNumber = studentNumber,
                            Semester = 1,
                            Status = STUDENT_STATUS.UNPAID,
                            IsRegistered = false
                        };
                        studentDB.SaveStudent(student);
                        await userManager.AddToRoleAsync(studentUser, "Student");
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        foreach (IdentityError error in studentResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View(userModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFaculty(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                CourseHelperUser user = new CourseHelperUser
                {
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    UserName = userModel.UserName
                };
                IdentityResult result = await userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Faculty");
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(userModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult List()
        {
            return View("UserList", userManager.Users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            CourseHelperUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                //Deletes Student class linked to account
                Student student = studentDB.Students.First(s => s.StudentNumber == user.StudentNumber);
                if (student != null)
                {
                    studentDB.DeleteStudent(student.StudentId);
                }
                //Deletes Account
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                //Need tempdata implementation
                //ModelState.AddModelError("", "User Not Found");
            }
            return View("List", userManager.Users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            CourseHelperUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                //Need tempdata impementation
                return RedirectToAction("List");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string firstName, string lastName,
        string password)
        {
            CourseHelperUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager,
                    user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user,
                        password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
                if (!string.IsNullOrEmpty(firstName))
                {
                    user.FirstName = firstName;
                    user.LastName = lastName;
                }
                if (validPass == null || (password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        Student student = studentDB.Students.First(s => s.StudentNumber == user.StudentNumber);
                        if (student != null)
                        {
                            student.FirstName = user.FirstName;
                            student.LastName = user.LastName;
                            studentDB.SaveStudent(student);
                        }
                        //Need tempdata impementation
                        return RedirectToAction("List");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                //Need tempdata implementation
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }
        
        [Authorize(Roles = "Student, Faculty")]
        public async Task<IActionResult> ChangePassword(string id)
        {
            CourseHelperUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                //Need tempdata impementation
                return RedirectToAction("List");
            }
        }
        
        [Authorize(Roles = "Student, Faculty")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, string oldPassword, string newPassword)
        {
            CourseHelperUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult changePass = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (changePass.Succeeded)
                {
                    //Need tempdata implementation
                    return RedirectToAction("Home", "Student");
                }
                else
                {
                    AddErrorsFromResult(changePass);
                }
            }
            else
            {
                //Need tempdata implementation
                ModelState.AddModelError("", "User Not Found");
            }
            return View("ChangePassword", id);
        }

        [AllowAnonymous]
        public IActionResult Login(string returnURL)
        {
            ViewBag.returnURL = returnURL;
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnURL)
        {
            if (ModelState.IsValid)
            {
                CourseHelperUser user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnURL ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.UserName), "Invalid user or password");
            }
            return View(model);
        }
        

        [Authorize]
        public async Task<IActionResult> Logout(string returnURL)
        {
            await signInManager.SignOutAsync();
            return Redirect(returnURL ?? "/");

        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}