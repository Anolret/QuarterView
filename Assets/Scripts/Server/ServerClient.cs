using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using UserDataDLL;

public class ServerClient {
	public UserData user = new UserData();

	private TcpClient client;
	string IP;
	int Port;

	/// <summary>
	/// -1 : 메세지 받는 중, 0 : false, 1 : true
	/// </summary>
	public int Isbool = -1;

	public bool IsScenes = false;

	//58.237.141.203 - 127.0.0.1
	public ServerClient (string ip = "127.0.0.1", int port = 9999) {
		IP = ip;
		Port = port;
	}

	public void Setting(string ip, int port) {
		IP = ip;
		Port = port;
	}

	void Connect() {
		client = new TcpClient();
		client.Connect(IP, Port);
	}

	//서버에 메시지 보내기
	public void DataMessage () {
		Isbool = -1;

		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();
		byte[] byteData;
		
		try {
			//서버 접속
			Connect();

			//데이터 전송
			bf.Serialize(ms, user);
			byteData = new byte[ms.Length];
			byteData = ms.GetBuffer();
			client.GetStream().Write(byteData, 0, byteData.Length);

			//서버에서 받았는지 확인
			string receiveMessage = "";
			byte[] receiveByte = new byte[1024];
			client.GetStream().Read(receiveByte, 0, receiveByte.Length);

			receiveMessage = Encoding.Default.GetString(receiveByte).Replace("\0", "");

			Isbool = int.Parse(receiveMessage);

			switch (user.Number) {
				case 0://로그인
					if (1 < Isbool) {
						//크기 받았다고 확인 메세지
						byteData = Encoding.Default.GetBytes("1");
						client.GetStream().Write(byteData, 0, byteData.Length);
					
						//받을 데이터 크기 지정
						receiveByte = new byte[Isbool];
						client.GetStream().Read(receiveByte, 0, receiveByte.Length);

						client.Close();

						//receiveByte 받아온 데이터 정리
						MemoryStream memoryStream = new MemoryStream(receiveByte);
						user = (UserData) bf.Deserialize(memoryStream);

						GameMaster.User = user;
						//<-
					} else {
						ScenesManager.Instance.InputCheck("가입하지 않은 아이디이거나, 잘못된 비밀번호입니다");
						
					}
					break;
				case 1://아이디 중복 확인
					if (Isbool == 1) {
						ServerManager.Instance.IsID = true;
					} else {
						ScenesManager.Instance.InputCheck("아이디가 중복입니다.");
					}
					break;
				case 2://회원가입
					if (Isbool == 1) {
						//가입 성공
						ScenesManager.Instance.SignUPwindow.SetActive(false);
					} else {
						//가입 실패
						ScenesManager.Instance.InputCheck("가입에 실패하였습니다.");
					}
					break;
				case 3://데이터 저장
					if (Isbool == 1) {
						
					} else {
						Debug.Log("저장 실패");
					}
					break;
			}
		} catch (Exception e) {
			Debug.Log("DataMessage(ex) : " + e);
		}
		
		Debug.Log("Thread END");
		user = new UserData();

		if (user.Number == 0 && 1 < Isbool) {
			IsScenes = true;
		}
	}
}
