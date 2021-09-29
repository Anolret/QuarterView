using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserDataDLL;

public class GameMaster : MonoBehaviour {
	public static GameMaster Instance;
	public static UserData User;

	public static bool localplay = false;

	public GameObject m_ExtinctionImpact;

	FileStream m_FileStream;

	public Image OptionButton;
	public GameObject OptionButtons;

	public Text[] StatusText;

	public bool IsLoad = false;

	public GameObject EndBG;

	public void Renew() {
		StatusText[0].text = ((int)PlayerInformation.m_PlayerStats.m_Status[StatusConstant.HP].GetChange()).ToString();
		StatusText[1].text = ((int)PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MP].GetChange()).ToString();
		StatusText[2].text = ((int)PlayerInformation.m_PlayerStats.m_Status[StatusConstant.ATK].GetChange()).ToString();
		StatusText[3].text = ((int)PlayerInformation.m_PlayerStats.m_Status[StatusConstant.ATKSpeed].GetChange()).ToString();
		StatusText[4].text = ((int)PlayerInformation.m_PlayerStats.m_Status[StatusConstant.DEF].GetChange()).ToString();
		StatusText[5].text = ((int)PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MoveSpeed].GetChange()).ToString();
	}

	public static GameObject ObjInformation() {
		GameObject _s = new GameObject();
		GameObject _obj = Instantiate(_s, new Vector3(), Quaternion.LookRotation(Vector3.forward));
		return _obj;
	}

	void Awake() {
		Instance = this;
		SceneManager.LoadScene("Field", LoadSceneMode.Additive);

		//SceneManager.UnloadSceneAsync(2); //불러온 씬 삭제
		//Time.timeScale = 0.5f;

		//몬스터 사망 임팩트 로드(지우지마!!!)
		//m_ExtinctionImpact = Resources.Load<GameObject>("Impact/Monster/ExtinctionImpact");
	}

	void Start() {
		Monster.m_Player = PlayerInformation.m_PlayerController.m_CenterPos;
	}


	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace)) {
			EndBG.SetActive(!EndBG.activeSelf);
		}
	}

	public void End () {
		Application.Quit();
	}

	public void DataSave() {
		BinaryFormatter bf = new BinaryFormatter();
		m_FileStream = File.Create(Application.persistentDataPath+"/PlayerData.sav");
		//-----

		User.data.Gold = ItemManager.Instance.Gold;

		SlotDataSave(User.data.EquipmentSlot, ItemManager.cEquipment);
		SlotDataSave(User.data.InventorySlot, ItemManager.cInventory);

		for (int i = 0 ; i < SkillManager.Instance.WinSkillSlot.Length ; i++) {
			if (SkillManager.Instance.WinSkillSlot[i] != null) {
				User.data.Skills[i] = SkillManager.Instance.WinSkillSlot[i].SkillNum;
			} else {
				User.data.Skills[i] = -1;
			}
		}

		//-----
		bf.Serialize(m_FileStream, User);
		m_FileStream.Close();
	}

	public void SlotDataSave(ItemData[] itemdatas, Item[] Items) {
		for (int i = 0 ; i < itemdatas.Length ; i++) {
			if (Items[i] != null) {
				itemdatas[i].Main = Items[i].GetMainCode();
				if (Items[i].GetMainCode() == ItemCode.Expendables) {
					itemdatas[i].ItemNum = ItemManager.m_Consumables.FindIndex(itme => itme.GetName() == Items[i].GetName());
				} else if (Items[i].GetMainCode() == ItemCode.Equipment) {
					itemdatas[i].ItemNum = ItemManager.m_Equipment.FindIndex(itme => itme.GetName() == Items[i].GetName());
				}
				itemdatas[i].Amount = Items[i].GetAmount();
			} else {
				itemdatas[i].Main = -1;
			}
		}
	}

	public void DataLoad() {
		try {
			m_FileStream = File.Open(Application.persistentDataPath + "/PlayerData.sav", FileMode.Open);
			
			if (m_FileStream != null && 0 < m_FileStream.Length) {
				IsLoad = true;
				BinaryFormatter bf = new BinaryFormatter();
				User = (UserData) bf.Deserialize(m_FileStream);

				ItemManager.Instance.Gold = User.data.Gold;

				SlotDataLoad(User.data.EquipmentSlot, ItemManager.cEquipment);
				SlotDataLoad(User.data.InventorySlot, ItemManager.cInventory);

				for (int i = 0 ; i < User.data.Skills.Length ; i++) {
					if (User.data.Skills[i] != -1) {
						SkillManager.Instance.Enrollment(i, User.data.Skills[i]);
					}
				}

				ItemManager.Instance.RenewalEquipment();

				PlayerInformation.m_PlayerStats.ReceiveData(StatusConstant.HP, PlayerInformation.m_PlayerStats.m_Status[StatusConstant.HP].GetChange());
				PlayerInformation.m_PlayerStats.ReceiveData(StatusConstant.MP, PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MP].GetChange());
			}
			m_FileStream.Close();
		} catch (IOException) {
			if (m_FileStream == null) {
				Debug.Log("파일 없음");
			} else {
				Debug.Log("불러오기 실패");
			}
			return;
		}
	}
	
	public void SlotDataLoad(ItemData[] itemdatas, Item[] Items) {
		for (int i = 0 ; i < itemdatas.Length ; i++) {
			if (itemdatas[i].Main != -1) {
				if (itemdatas[i].Main == ItemCode.Expendables) {
					Items[i] = new Item(ItemManager.m_Consumables[itemdatas[i].ItemNum]);
				} else if (itemdatas[i].Main == ItemCode.Equipment) {
					Items[i] = new Item(ItemManager.m_Equipment[itemdatas[i].ItemNum]);
				}
				Items[i].SetAmount(itemdatas[i].Amount);
			} else {
				Items[i] = null;
			}
		}
	}

	public void ServerSave () {
		if (!localplay) {
			if (User != null) {
				User.Number = 3;
				User.data.Gold = ItemManager.Instance.Gold;

				SlotDataSave(User.data.EquipmentSlot, ItemManager.cEquipment);
				SlotDataSave(User.data.InventorySlot, ItemManager.cInventory);

				for (int i = 0 ; i < SkillManager.Instance.WinSkillSlot.Length ; i++) {
					if (SkillManager.Instance.WinSkillSlot[i] != null) {
						User.data.Skills[i] = SkillManager.Instance.WinSkillSlot[i].SkillNum;
					} else {
						User.data.Skills[i] = -1;
					}
				}

				ServerManager.ServerClient.user = User;

				ServerManager.ServerClient.DataMessage();
			} else {
				Debug.Log("저장 실패");
			}
		} else {
			User = new UserData();
			DataSave();
		}
	}
}
