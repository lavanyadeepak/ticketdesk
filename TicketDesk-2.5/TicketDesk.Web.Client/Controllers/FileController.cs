﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TicketDesk.IO;

namespace TicketDesk.Web.Client.Controllers
{
    public class FileController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Upload(Guid tempId)
        {
            //TODO: doesn't necessarily play well with large files, cap file upload size to something reasonable (define in settings) or use upload chunking
            foreach (string fileName in Request.Files)
            {
                var file = Request.Files[fileName];
                Debug.Assert(file != null, "file != null");
                await TicketDeskFileStore.SaveAttachmentAsync(file.InputStream, file.FileName, tempId.ToString(), true);
            }
            return Json(new { Message = string.Empty });//dropzone expects a message property back
        }

        [HttpPost]
        public ActionResult Delete(Guid tempId, string fileName)
        {
            TicketDeskFileStore.DeleteAttachment(fileName, tempId.ToString(), true);
            return new JsonCamelCaseResult { Data = new { Message = string.Empty } };//dropzone expects a message property back
        }

        public ActionResult GetFile()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult GetPendingAttachmentsInfo(Guid tempId)
        {
            var files = TicketDeskFileStore.ListAttachmentInfo(tempId.ToString(), true);
            return new JsonCamelCaseResult()
            {
                Data = files.Select(f => new { f.Name, f.Size, Url = Url.Action("GetFile") }),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


    }
}