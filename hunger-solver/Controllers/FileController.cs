using Firebase.Auth;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace hunger_solver.Controllers
{
    public class FileController : Controller
    {
        private static string ApiKey = "AIzaSyDmBVtxgVhAJUokqpqid2UC2Gwc3gRsGG8 ";
        private static string Bucket = "hunger-solver-3237d.appspot.com";
        private static string AuthMail = "techtoon526628@gmail.com";
        private static string AuthPass = "123456";

        [HttpPost]
        public async Task<ActionResult> UploadImage(HttpPostedFileBase file)
        {
            FileStream stream;
            if(file.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/images/"), file.FileName);
                file.SaveAs(path);
                stream = new FileStream(Path.Combine(path), FileMode.Open);
                await Task.Run(() => Upload(stream, file.FileName));
            }
            return RedirectToAction("/PersonalInfo");
        }

        public async void Upload(FileStream stream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthMail, AuthPass);

            //cancel at mid
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when cancel the upload, exception is thrown
                })
                .Child("images")
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);
            try
            {
                string link = await task;
            }catch(Exception ex)
            {
                Debug.WriteLine("Exception from file upload " + ex);
            }
        }
        
    }
}