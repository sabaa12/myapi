using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apitesthost.Models;
using apitesthost.Tools;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using apitesthost.Models.finalproject;
using apitesthost.Models.finalproject.CompleteProfile;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json.Linq;

namespace apitesthost.Controllers
{
    [Route("api/")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly MvcMovieContext _context;

        public usersController(MvcMovieContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Users/{id}")]
        public async Task<ActionResult<compteteprofilepost>> GetUsers(int id)
        {
            if (!_context.users.Where(x => x.ID == id).FirstOrDefault().iscomplete)
            {
                var error = new errormodel();
                error.error = "profile is not completed";
                return NotFound(error);
            }
            var model = new compteteprofilepost();
            var compmodel = _context.complete_profile.Where(x => x.ID == id).FirstOrDefault();
            model.user_name = compmodel.user_name;
            model.email_address = compmodel.email_address;
            model.gender = compmodel.gender;
            model.age  = compmodel.age.ToString();
            model.position = _context.developer.Where(x => x.ID == id).FirstOrDefault().position;
            var role = _context.users.Where(x => x.ID == id).FirstOrDefault().role;
            if (role == 1) model.role = "Developer";
            var skills = _context.skkils.Where(x => x.developerID == id).ToList();
            var list = new List<string>();
            foreach (var item in skills)
            {
                list.Add(item.skill.Trim());
            }
            model.skills = list;
            return model;
        }


        [HttpGet]
        [Route("isusercomplete/{id}")]
        public async Task<ActionResult<isprofilecompletedmodel>> UsersComplete(int id)
        {
            var complete = new isprofilecompletedmodel();
            complete.iscompleted = _context.users.Where(x => x.ID == id).FirstOrDefault().iscomplete;
            return complete;
        }
        
        [HttpPost]
        [Route("signUp")]
        public async Task<ActionResult<signUpreturnvalues>> signUp([FromForm]userModel user)
        {
             
            var returnvalues = new signUpreturnvalues();
            var mail = _context.users.Where(x => x.email_address == user.email_address).FirstOrDefault();
            if ( mail!= null)
            {
                var error = new errormodel();
                error.error = "E-MAil allready used";
                return Unauthorized(error);
            }
            var suser = new users();
            suser.email_address = user.email_address;
            suser.password = tools.hashpassword(user.password);
            if (user.role.ToLower() == "developer") suser.role = 1;
            else if(user.role.ToLower() == "employer") suser.role = 2;
            suser.iscomplete = false;
             
             _context.users.Add(suser);
            await _context.SaveChangesAsync();
            
            
            returnvalues.Registered = true;
            returnvalues.UserID = suser.ID.ToString();
            returnvalues.Status = "user was created successfully";
          return returnvalues;
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<ActionResult<SignInreturnvalues>> SignIn([FromForm]string email_address,[FromForm]string password)
        {
            var ret = new SignInreturnvalues();
            var user = _context.users.Where(x => x.email_address == email_address).FirstOrDefault();
            if (user != null)
            {
                if (user.password == tools.hashpassword(password))
                {
                    ret.Status = "ok";
                    ret.UserID = user.ID.ToString();
                    return ret;
                }
                else
                {
                    ret.Status = "invalid password";
                    return NotFound(ret);
                }
            }
            ret.Status = "invalid E-MAIL";

            return NotFound(ret);
        }

        //[HttpPost]
        //[Route("CompleteProfile")]
        //public async Task<ActionResult<completereturnvalue>> CompleteProfile([FromBody] compteteprofilepost complete)
        //{
        //    var model = new complete_profile();
        //    var developermodel = new developer();

        //    var userID = _context.users.Where(x => x.email_address == complete.email_address).FirstOrDefault().ID;
        //    model.user_name = complete.user_name;
        //    model.email_address = complete.email_address;
        //    model.gender = complete.gender;
        //    model.age = Int32.Parse(complete.age);
        //    model.ID = userID;
        //    model.photo_url = complete.photo_url;
        //    _context.complete_profile.Add(model);

        //    developermodel.ID = userID;
        //    developermodel.position = complete.position;

        //    _context.developer.Add(developermodel);
        //    foreach (var item in complete.skills)
        //    {
        //        var skillmodel = new skkils();
        //        skillmodel.developerID = userID;
        //        skillmodel.skill = item;
        //        _context.skkils.Add(skillmodel);
        //    }
        //    await _context.SaveChangesAsync();

        //    _context.users.Where(x => x.email_address == complete.email_address).FirstOrDefault().iscomplete = true;
        //    var ret = new completereturnvalue();
        //    ret.profile_completed = true; 
        //    await _context.SaveChangesAsync();

        //    return ret;
        //}

        [HttpPost]
        [Route("CompleteProfile")]
        public async Task<ActionResult<completereturnvalue>> CompleteProfile([FromForm] getjsnoasstring complete1)
        {
            var complete = Newtonsoft.Json.JsonConvert.DeserializeObject<compteteprofilepost>(complete1.json);

            var model = new complete_profile();
            var developermodel = new developer();
            var email = _context.users.Where(x => x.email_address == complete.email_address).FirstOrDefault();
            if ( email == null)
            {
                var error = new errormodel();
                error.error = "user with this email doesnot exisit";
                return NotFound(error);
            }
            else if(_context.users.Where(x => x.email_address == complete.email_address).FirstOrDefault().iscomplete)
            {
                var error = new errormodel();
                error.error = "This account  is allready completed";
                return Unauthorized(error);
            }
            var userID = _context.users.Where(x => x.email_address == complete.email_address).FirstOrDefault().ID;
            model.user_name = complete.user_name;
            model.email_address = complete.email_address;
            model.gender = complete.gender;
            model.age = Int32.Parse(complete.age);
            model.ID = userID;
            model.photo_url = complete.photo_url;
            _context.complete_profile.Add(model);

            developermodel.ID = userID;
            developermodel.position = complete.position;

            _context.developer.Add(developermodel);
            foreach (var item in complete.skills)
            {
                var skillmodel = new skkils();
                skillmodel.developerID = userID;
                skillmodel.skill = item;
                _context.skkils.Add(skillmodel);
            }
            await _context.SaveChangesAsync();

            _context.users.Where(x => x.email_address == complete.email_address).FirstOrDefault().iscomplete = true;
            var ret = new completereturnvalue();
            ret.profile_completed = true;
            await _context.SaveChangesAsync();

            return ret;
        }

    }

      
}
