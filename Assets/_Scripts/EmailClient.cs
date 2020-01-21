using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class EmailClient
{
	private SmtpClient client;
	private string email;
	private string password;

	public EmailClient(string domain, int port, string email, string password)
	{
		client = new SmtpClient(domain, port);
		client.Credentials = new System.Net.NetworkCredential(email, password);
		client.EnableSsl = true;
		this.email = email;
		this.password = password;
	}

	public void SendMessage(string reciever, string subject, string message)
	{
		MailMessage msg = new MailMessage();
		msg.From = new MailAddress(email);
		msg.To.Add(new MailAddress(reciever));
		msg.Subject = subject;
		msg.Body = message;
		client.Send(msg);
	}
}
