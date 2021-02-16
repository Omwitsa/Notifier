using System;

namespace AbnNotifier.Services.Email
{
    public class Template
    {
        public static string Get(string subject, string name, string content)
        {
            var year = DateTime.UtcNow.Year;

            var message = "<div style='margin: 2em 5em 2em 5em; background-color: #f2f2f2'>" +
                          "<table style='width: 100 %; margin: 5% 10% 5% 10%;'><br>" +
                          //"<tr><td><img src='cid:logoId' style='width:200px; display: block; margin-left: auto; margin-right: auto;'/></td></tr>" +
                          "<tr><td><h2 style='color: red'>" + subject + "<br></h2></td></tr>" +
                          "<tr><td><h4>Dear " + name + ",</h4></td></tr>" +
                          "<tr><td>" + content + "<br> <br></td></tr>" +
                          "<tr><td><p><span style='font-weight: bold'> Disclaimer:- </span> <i>The content of this email is confidential and intended for the recipient specified in this message only. " +
                          "It is strictly forbidden to share any part of this message with any third party. " +
                          "If you received this message by mistake, please reply to this message and follow with its deletion, " +
                          "so that we can ensure such a mistake does not occur in the future.</i> </p></td></tr>" +
                          "</table>" +
                          "<p style='text-align: center'>Powered By <a href='http://www.abnosoftwares.co.ke/' target='_blank' style='color:blue;'>" +
                          "<b>ABNO Softwares International Ltd.</b></a> &copy; Copyright <span>" + year + " </span></p> <br>" +
                          "</div>";
            return message;

        }
    }
}
