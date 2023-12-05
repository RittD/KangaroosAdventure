using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MailSender : MonoBehaviour
{
    public void SendEmail()
    {
        string email = "support_knightstudios@mail.de";
        string subject = MyEscapeURL("Inquiry concerning Kangaroo's Adventures");
        string body = "";
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url) => UnityWebRequest.EscapeURL(url).Replace("+", "%20");
}
