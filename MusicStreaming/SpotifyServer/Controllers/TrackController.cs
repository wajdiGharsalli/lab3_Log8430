using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Utilities;

namespace SpotifyServer.Controllers
{
    public class TrackController : ApiController
    {
        // GET /Track?keys=words
        public async Task<JsonResult<List<LocalTrack>>> Get(string keys)
        {
            if (keys == null || keys == string.Empty)
                return null;
            return Json(await SpotifySystemManager.GetInstance().SearchTrack(keys));
        }

        // POST: /Track
        public void Post([FromBody]string value)
        {
        }

        // PUT: /Track/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: /Track/5
        public void Delete(int id)
        {
        }
    }
}
