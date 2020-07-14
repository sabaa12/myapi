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
            var role = _context.users.Where(x => x.ID == id).FirstOrDefault().role;

            if (role != 1)
            {
                var error = new errormodel();
                error.error = "this user is not Developer";
                return NotFound(error);

            }

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
            model.ID = id;
            model.age  = compmodel.age.ToString();
            model.position = _context.developer.Where(x => x.ID == id).FirstOrDefault().position;
            model.photo_url = compmodel.photo_url;
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
        [Route("GetEmployerUsersPerson/{id}")]
        public async Task<ActionResult<emploergetmodel>> Getemployerusers(int id)
        {
            var role = _context.users.Where(x => x.ID == id).FirstOrDefault().role;
        
            if (role != 2)
            {
                var error = new errormodel();
                error.error = "this user is not employer";
                return NotFound(error);

            }
            if (_context.complete_profile.Where(c => c.ID == id).FirstOrDefault() == null)
            {
                var error = new errormodel();
                error.error = "this employer is not person";
                return NotFound(error);

            }
            if (!_context.users.Where(x => x.ID == id).FirstOrDefault().iscomplete)
            {
                var error = new errormodel();
                error.error = "profile is not completed";
                return NotFound(error);
            }

            var model = new emploergetmodel();
            var compmodel = _context.complete_profile.Where(x => x.ID == id).FirstOrDefault();
            var comanymodel = _context.employer_company.Where(x => x.ID == id).FirstOrDefault();
            model.user_name = compmodel.user_name;
            model.email_address = compmodel.email_address;
            model.gender = compmodel.gender;
            model.age = compmodel.age;
            model.photo_url = compmodel.photo_url;
            model.role = "Employer";
           
            
           
             
            
            return model;
        }

        [HttpGet]
        [Route("GetEmployerUsersCompany/{id}")]
        public async Task<ActionResult<employer_company>> GetemployerusersCompany(int id)
        {
            var role = _context.users.Where(x => x.ID == id).FirstOrDefault().role;
            if (role != 2)
            {
                var error = new errormodel();
                error.error = "this user is not employer";
                return NotFound(error);

            }
            if (_context.employer_company.Where(c => c.ID == id).FirstOrDefault() == null)
            {
                var error = new errormodel();
                error.error = "this employer is not company";
                return NotFound(error);

            }
            if (!_context.users.Where(x => x.ID == id).FirstOrDefault().iscomplete)
            {
                var error = new errormodel();
                error.error = "profile is not completed";
                return NotFound(error);
            }
            return _context.employer_company.Where(x => x.ID == id).FirstOrDefault();
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
            suser.email_address = user.email_address;
            suser.iscomplete = false;
             
             _context.users.Add(suser);
            await _context.SaveChangesAsync();
            
            
            returnvalues.Registered = true;
            returnvalues.UserID = suser.ID.ToString();
            returnvalues.Status =true;
          return returnvalues;
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<ActionResult<SignInreturnvalues>> SignIn([FromForm]string email_address,[FromForm]string password)
        {
            var error = new errormodel();
            var ret = new SignInreturnvalues();
            var user = _context.users.Where(x => x.email_address == email_address).FirstOrDefault();
            if (user != null)
            {
                
                if (user.password == tools.hashpassword(password))
                {
                    var uid = _context.users.Where(x => x.email_address == email_address).FirstOrDefault().ID;

                    ret.Status = true;
                    ret.UserID = user.ID.ToString();
                    var roleid= _context.users.Where(x => x.email_address == email_address).FirstOrDefault().role;
                    if (roleid == 1)
                    {
                        ret.role = "Developer";
                        ret.Photo_url = _context.complete_profile.Where(x => x.ID == uid).FirstOrDefault().photo_url;
                    }

                    else if (roleid == 2)
                    {
                        ret.role = "Employer";
                        
                        if (_context.users.Where(x => x.email_address == email_address).FirstOrDefault().iscomplete)
                        {
                            if (_context.employer_company.Where(x => x.ID == uid).FirstOrDefault() != null)
                            {
                                ret.employertype = "Company";
                                ret.Photo_url = _context.employer_company.Where(x => x.ID == uid).FirstOrDefault().company_logo;
                            }
                            else
                            {
                                ret.Photo_url = _context.complete_profile.Where(x => x.ID == uid).FirstOrDefault().photo_url;
                                ret.employertype = "Person";
                            }

                        }


                    }
                    ret.email_address = email_address;
                    return ret;
                }
                else
                {
                   
                    error.error = "password is incorrect";
                    return NotFound(error);
                }
            }
            error.error = "email is incorrect";
            return NotFound(error);
        }

         
        [HttpPost]
        [Route("СompleteProfileDeveloper")]
        public async Task<ActionResult<completereturnvalue>> СompleteProfileDeveloper([FromForm] getjsnoasstring complete1)
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



        [HttpPost]
        [Route("СompleteProfileEmployerPerson")]
        public async Task<ActionResult<completereturnvalue>> CompleteProfile_empleyers([FromForm] employersModel complete1)
        {
            var perosnmodel = new complete_profile();
            if (_context.users.Where(x => x.email_address == complete1.email_address).FirstOrDefault() == null)
            {
                var error = new errormodel();
                error.error = "user with this email doesnot exisit";
                return NotFound(error);
            }
            var userID = _context.users.Where(x => x.email_address == complete1.email_address).FirstOrDefault().ID;
            if (_context.users.Where(x => x.ID == userID).FirstOrDefault().role != 2)
            {
                var error = new errormodel();
                error.error = "this user is not Employer";
                return NotFound(error);
            }
       
            var companymodel = new employer_company();

            perosnmodel.ID = userID;
            perosnmodel.photo_url = complete1.photo_url;
            perosnmodel.user_name = complete1.user_name;
            perosnmodel.email_address = complete1.email_address;
            perosnmodel.gender = complete1.gender;
            perosnmodel.age = Int32.Parse(complete1.age);
           
            _context.complete_profile.Add(perosnmodel);

            var ret = new completereturnvalue();
            _context.users.Where(x => x.email_address == complete1.email_address).FirstOrDefault().iscomplete = true;
            ret.profile_completed = true;
            await _context.SaveChangesAsync();
            return ret ;
        }


        [HttpPost]
        [Route("СompleteProfileEmployerCompany")]
        public async Task<ActionResult<completereturnvalue>> CompleteProfileEmpleyersCompany([FromForm] comanyModel complete1)
        {
            var comanyodel = new employer_company();
            if (_context.users.Where(x => x.email_address == complete1.email_address).FirstOrDefault() == null)
            {
                var error = new errormodel();
                error.error = "user with this email doesnot exisit";
                return NotFound(error);
            }
            var userID = _context.users.Where(x => x.email_address == complete1.email_address).FirstOrDefault().ID;
            if (_context.users.Where(x => x.ID == userID).FirstOrDefault().role != 2)
            {
                var error = new errormodel();
                error.error = "this user is not Employer";
                return NotFound(error);
            }

            comanyodel.company_logo = complete1.company_logo;
            comanyodel.company_name = complete1.company_name;
            comanyodel.ID = userID;
            _context.employer_company.Add(comanyodel);

            try
            {
                await  _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var error = new errormodel();
                error.error = ex.Message;
                return NotFound(error);

            }
            var ret = new completereturnvalue();
            _context.users.Where(x => x.email_address == complete1.email_address).FirstOrDefault().iscomplete = true;
            ret.profile_completed = true;
            await _context.SaveChangesAsync();
            return ret;
        }
         

        [HttpPost]
        [Route("CreatePost")]
        public async Task<ActionResult<status>> CreatePost([FromForm] getjsnoasstring complete1)
        {
            var post = JsonConvert.DeserializeObject<CreatePostModel>(complete1.json);
            var model =new  create_post();
 
            model.create_date = post.create_date;
            model.description = post.description;
            model.EmployerID = post.ID;
            model.title = post.title;
            model.experience_level = post.experience_level;
            _context.Create_Post.Add(model);
            await _context.SaveChangesAsync();
            var postId = model.ID;
            foreach (var item in post.skills)
            {
                var skillsModel = new developer_skills();
                skillsModel.employerID = post.ID;
                skillsModel.skill = item;
                skillsModel.PostID = postId;
                _context.developer_Skills.Add(skillsModel);
            } 
        
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var error = new errormodel();
                error.error = ex.Message;
                return Unauthorized(error);
            }

            var ret = new status();
            ret.ispostcreated =true;


            return ret ;
        }

        [HttpGet]
        [Route("GetPosts")]
         public async Task<ActionResult<List<CreatePostModel>>> GetPosts()
        {
            var list = _context.Create_Post.ToList();
            
            var retlist = new List<CreatePostModel>();
            foreach (var item in list)
            {
                var list2 = new List<string>();
                var postmodel = new CreatePostModel();
                postmodel.create_date = item.create_date;
                postmodel.description = item.description;
                postmodel.experience_level = item.experience_level.Trim();
                postmodel.ID = item.EmployerID;
                postmodel.title = item.title;
                postmodel.postID = item.ID;
                postmodel.email_address = _context.users.Where(x => x.ID == item.EmployerID).FirstOrDefault().email_address;
                if (_context.employer_company.Where(x => x.ID == item.EmployerID).FirstOrDefault() != null)
                    postmodel.photo_url = _context.employer_company.Where(x => x.ID == item.EmployerID).FirstOrDefault().company_logo;
                else 
                postmodel.photo_url = _context.complete_profile.Where(c => c.ID == item.EmployerID).FirstOrDefault().photo_url;
                 foreach (var item2 in _context.developer_Skills.Where(x => x.PostID == item.ID).ToList())
                {
                   
                    list2.Add(item2.skill.Trim());

                }
                postmodel.skills = list2;
                retlist.Add(postmodel);
            }

            return retlist;
        }

        [HttpGet]
        [Route("GetDevelopers")]
        public async Task<ActionResult<List<compteteprofilepost>>> GetDevelopers()
        {
            var list = _context.users.Where(x => x.role == 1).ToList();
            var returnlist = new List<compteteprofilepost>();
            foreach (var item in list)
            {
                var uid = item.ID;
                var model = new compteteprofilepost();
                if (item.iscomplete)
                {
                    var comleteProfieModel = _context.complete_profile.Where(x => x.ID == uid).FirstOrDefault();
                    model.email_address = item.email_address;
                    model.user_name = comleteProfieModel.user_name;
                    model.gender = comleteProfieModel.gender;
                    model.age = comleteProfieModel.age.ToString();
                    model.photo_url = comleteProfieModel.photo_url;
                    model.position= _context.developer.Where(x => x.ID == uid).FirstOrDefault().position;
                    model.role = "Developer";
                    model.ID = uid;
                    var skills = _context.skkils.Where(x => x.developerID == uid).ToList();
                    var skilllist = new List<string>();
                    foreach (var item2 in skills)
                    {
                        skilllist.Add(item2.skill.Trim());
                    }
                    model.skills = skilllist;

                    returnlist.Add(model);
                }

            }
              

            return returnlist;
        }


        [HttpPost]
        [Route("addDeveloperToFavorites")]
        public async Task<ActionResult<status>> addToFavorites([FromForm]favoritesModel model)
        {
            var datamodel = new favorites();
            datamodel.user_id = model.user_id;
            datamodel.favorited_user_id = model.favorited_user_id;
            _context.favorites.Add(datamodel);
            await _context.SaveChangesAsync();

            var ret = new status();
            ret.ispostcreated = true;

            return ret;
        }

        [HttpGet]
        [Route("GetFavoriteDevelopers/{uid}")]
        public async Task<ActionResult<List<compteteprofilepost>>> Getfavorites(int uid)
        {
            var listt = _context.favorites.Where(x => x.user_id == uid).ToList();
            var returnlist = new List<compteteprofilepost>();
            var id = 0;
            foreach (var item in listt)
            {
                id = item.favorited_user_id;

                var model = new compteteprofilepost();
                var compmodel = _context.complete_profile.Where(x => x.ID == id).FirstOrDefault();
                model.user_name = compmodel.user_name;
                model.email_address = compmodel.email_address;
                model.gender = compmodel.gender;
                model.age = compmodel.age.ToString();
                model.ID = id;
                model.position = _context.developer.Where(x => x.ID == id).FirstOrDefault().position;
                model.photo_url = compmodel.photo_url;
                var skills = _context.skkils.Where(x => x.developerID == id).ToList();
                var list = new List<string>();
                foreach (var item2 in skills)
                {
                    list.Add(item2.skill.Trim());
                }
                model.skills = list;
                returnlist.Add(model);
            }


            return returnlist;
        }


        [HttpPost]
        [Route("addPostToFavorites")]
        public async Task<ActionResult<status>> addPostToFavorites([FromForm] favoritesModel model)
        {
            var datamodel = new favorite_posts();
            datamodel.developer_id = model.user_id;
            datamodel.post_id = model.favorited_user_id;
            _context.favorite_Posts.Add(datamodel);
            await _context.SaveChangesAsync();

            var ret = new status();
            ret.ispostcreated = true;

            return ret;

        }

        [HttpGet]
        [Route("GetFavoritePosts/{uid}")]
        public async Task<ActionResult<List<CreatePostModel>>> GetFavoritePosts(int uid)
        {
            var listt = _context.favorite_Posts.Where(x => x.developer_id == uid).ToList();
            var retlist = new List<CreatePostModel>();

            foreach (var item1 in listt)
            {
                var list = _context.Create_Post.Where(x => x.ID == item1.post_id).ToList();
                foreach (var item in list)
                {
                    var list2 = new List<string>();
                    var postmodel = new CreatePostModel();
                    postmodel.create_date = item.create_date;
                    postmodel.description = item.description;
                    postmodel.experience_level = item.experience_level.Trim();
                    postmodel.ID = item.EmployerID;
                    postmodel.title = item.title;
                    postmodel.postID = item.ID;
                    postmodel.email_address = _context.users.Where(x => x.ID == item.EmployerID).FirstOrDefault().email_address;
                    if (_context.employer_company.Where(x => x.ID == item.EmployerID).FirstOrDefault() != null)
                        postmodel.photo_url = _context.employer_company.Where(x => x.ID == item.EmployerID).FirstOrDefault().company_logo;
                    else
                        postmodel.photo_url = _context.complete_profile.Where(c => c.ID == item.EmployerID).FirstOrDefault().photo_url;
                    foreach (var item2 in _context.developer_Skills.Where(x => x.PostID == item.ID).ToList())
                    {

                        list2.Add(item2.skill.Trim());

                    }
                    postmodel.skills = list2;
                    retlist.Add(postmodel);
                }
            }

            return retlist;
        }

        [HttpPost]
        [Route("RemoveDeveloperFromFavorites")]
        public async Task<ActionResult<status>> RemoveDeveloperFromFavorites([FromForm] favoritesModel model)
        {
            var delmodel = _context.favorites.Where(c => c.user_id == model.user_id).Where(x => x.favorited_user_id == model.favorited_user_id).FirstOrDefault();

            _context.favorites.Remove(delmodel);
            await _context.SaveChangesAsync();

            var ret = new status();
            ret.ispostcreated = true;

            return ret;
        }

        [HttpPost]
        [Route("RemovePostFromFavorites")]
        public async Task<ActionResult<status>> RemovePostFromFavorites([FromForm] favoritesModel model)
        {
            var delmodel = _context.favorite_Posts.Where(c => c.developer_id == model.user_id).Where(x => x.post_id == model.favorited_user_id).FirstOrDefault();

            _context.favorite_Posts.Remove(delmodel);
            await _context.SaveChangesAsync();

            var ret = new status();
            ret.ispostcreated = true;

            return ret;
        }

        [HttpPost]
        [Route("checkDeveloper")]
        public async Task<ActionResult<Isfav>> checkDeveloper([FromForm] favoritesModel model)
        {
            var retmodel = new Isfav();
            var delmodel = _context.favorites.Where(c => c.user_id == model.user_id).Where(x => x.favorited_user_id == model.favorited_user_id).FirstOrDefault();
            if (delmodel == null) retmodel.ISFavorite = false;
            else retmodel.ISFavorite =true;
            return retmodel;
        }

        [HttpPost]
        [Route("chechPost")]
        public async Task<ActionResult<Isfav>> chechPost([FromForm] favoritesModel model)
        {
            var retmodel = new Isfav();
            var delmodel = _context.favorite_Posts.Where(c => c.developer_id == model.user_id).Where(x => x.post_id == model.favorited_user_id).FirstOrDefault();
            if (delmodel == null) retmodel.ISFavorite = false;
            else retmodel.ISFavorite = true;
            return retmodel;
        }

        [HttpPost]
        [Route("GetUsernameandPhotoUrl")]
        public async Task<ActionResult<USernamewithPhotoModel>> GetUsernameandPhotoUrl([FromForm]ReceiverSenderModel model)
        {
            var retmodel = new USernamewithPhotoModel();
            retmodel.senderPhotoUrl = _context.complete_profile.Where(x => x.ID == model.senderID).FirstOrDefault().photo_url;
            retmodel.receiverUSername = _context.complete_profile.Where(x => x.ID == model.receiverID).FirstOrDefault().user_name;
            retmodel.receiverPhotoUrl = _context.complete_profile.Where(x => x.ID == model.receiverID).FirstOrDefault().photo_url;

            return retmodel;
        }

    }
}
