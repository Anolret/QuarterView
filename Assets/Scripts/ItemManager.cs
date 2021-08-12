using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {
	public static ItemManager Instance;

	/// <summary>
	/// 소비템(원본)
	/// </summary>
	public static List<Item> m_Consumables = new List<Item>();
	/// <summary>
	/// 장비템(원본)
	/// </summary>
	public static List<Item> m_Equipment = new List<Item>();

	public static Item[] cInventory = new Item[30];
	public static Item[] cEquipment = new Item[12];
	
	/// <summary>
	/// 계속 생성해서 쓰는 스트링 방지
	/// </summary>
	public static StringBuilder Str = new StringBuilder();

	public Fielditem m_ItemObj;

	public GameObject m_InventoryBG;

	public Sprite m_ItemNull;
	public Sprite m_EquipmentNull;

	public static Sprite spItemNull;
	public static Sprite spEquipmentNull;

	public Dictionary<string, Sprite> m_ItemSprites = new Dictionary<string, Sprite>();

	public Sprite m_ItemImg;
	public Sprite m_PortionImg;
	public Sprite m_WeaponImg;
	public Sprite m_ArmorImg;
	public Sprite m_AccImg;

	static Sprite spItemImg;
	static Sprite spPortionImg;
	static Sprite spWeaponImg;
	static Sprite spArmorImg;
	static Sprite spAccImg;

	/// <summary>
	/// 아이템 정보 출력 Obj
	/// </summary>
	public RectTransform m_ItemDataBG;
	public Text m_ItemName;
	public Text m_ItemDataText;
	public Text m_ItemDataTextDum;
	/// <summary>
	/// 장비 아이템
	/// </summary>
	public RectTransform m_EquipmentDataBG;
	public Text m_EquipmentName;
	public Text m_EquipmentDataText;
	public Text m_EquipmentDataTextDum;

	public GameObject m_SellButton;

	public int DataGold;
	public int Gold {
		get { return DataGold; }
		set {
			DataGold = value;
			GoldText.text = DataGold.ToString();
		}
	}

	public Text GoldText;
	
	/// <summary>
	/// 판매
	/// </summary>
	public void Sell() {
		Gold += Inventory.selItem.GetGold() / 2;
		Inventory.ItemImages[Inventory.Index].sprite = spItemNull;
		Inventory.ItemDataBG.gameObject.SetActive(false);

		cInventory[Inventory.Index] = null;
		
		DataBGActive(false);
		IndesReset();
	}

	void Awake() {
		Instance = this;
		AwakeSprite();

		SetImg();
		ConsumablesSetting();
		EquipmentItem();
		
		spItemNull = m_ItemNull;
		spEquipmentNull = m_EquipmentNull;

		Inventory.ItemDataBG = m_ItemDataBG;
		Inventory.ItemName = m_ItemName;
		Inventory.ItemDataText = m_ItemDataText;
		Inventory.ItemDataTextDum = m_ItemDataTextDum;

		EquipmentInventory.EquipmentDataBG = m_EquipmentDataBG;
		EquipmentInventory.EquipmentName = m_EquipmentName;
		EquipmentInventory.EquipmentDataText = m_EquipmentDataText;
		EquipmentInventory.EquipmentDataTextDum = m_EquipmentDataTextDum;

		Instance.Gold = 1000;
	}

	void Start() {
		PlayerInformation.PlayerSetInfo();

		m_InventoryBG.SetActive(false);
		m_InventoryBG.SetActive(true);
		m_InventoryBG.SetActive(false);
	}

	void AwakeSprite() {
		List<Sprite> _sprite = new List<Sprite>();
		try {
			//_sprite.Add(Resources.Load<Sprite>(""));

			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/hp"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/mp"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/b_t_01"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/armor"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/belts"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/boots"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/gloves"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/helmets"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/pants"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/necklace"));
			_sprite.Add(Resources.Load<Sprite>("UI/RPG_inventory_icons/rings"));

			for (int i = 0 ; i < _sprite.Count ; i++) {
				m_ItemSprites.Add(_sprite[i].name, _sprite[i]);
			}
		} catch (System.Exception e) {
			Debug.Log(e);
		}
	}

	/// <summary>
	/// 이미지 세팅
	/// </summary>
	public void SetImg() {
		spItemImg = m_ItemImg;
		spPortionImg = m_PortionImg;
		spWeaponImg = m_WeaponImg;
		spArmorImg = m_ArmorImg;
		spAccImg = m_AccImg;
	}

	public void InventoryOpen() {
		m_InventoryBG.SetActive(true);
	}

	public Sprite GetItemImg(int _num = -1) {
		switch (_num) {
			case 0:
				return m_ItemSprites["hp"];
			case 1:
				return m_ItemSprites["mp"];
			case 2:
				return m_ItemSprites["b_t_01"];
			case 3:
				return m_ItemSprites["armor"];
			case 4:
				return m_ItemSprites["belts"];
			case 5:
				return m_ItemSprites["boots"];
			case 6:
				return m_ItemSprites["gloves"];
			case 7:
				return m_ItemSprites["helmets"];
			case 8:
				return m_ItemSprites["pants"];
			case 9:
				return m_ItemSprites["necklace"];
			case 10:
				return m_ItemSprites["rings"];
		}
		return spItemImg;
	}

	public void SelectRelease() {
		Inventory.selItem = null;
		EquipmentInventory.scItem = null;


	}

	/// <summary>
	/// 소비아이템
	/// </summary>
	void ConsumablesSetting() {
		m_Consumables.Add(new Portion("초급HP물약", 30, CodeType.OnePortion, StatusConstant.HP, 50, 100, "hp"));
		m_Consumables.Add(new Portion("초급MP물약", 30, CodeType.TwoPortion, StatusConstant.MP, 50, 100, "mp"));

		m_Consumables.Add(new Portion("중급HP물약", 30, CodeType.OnePortion, StatusConstant.HP, 100, 300, "hp"));
		m_Consumables.Add(new Portion("중급MP물약", 30, CodeType.TwoPortion, StatusConstant.MP, 100, 300, "mp"));

		m_Consumables.Add(new Portion("상급HP물약", 30, CodeType.OnePortion, StatusConstant.HP, 200, 500, "hp"));
		m_Consumables.Add(new Portion("상급MP물약", 30, CodeType.TwoPortion, StatusConstant.MP, 200, 500, "mp"));
	}
	//갑옷
	void ArmorSetting() {
		Armor _armor;
		_armor = new Armor("초급가죽투구", CodeType.Helmet, "helmets");
		_armor.AddStatus(StatusConstant.DEF, 3f);
		_armor.SetGold(40);
		m_Equipment.Add(_armor);

		_armor = new Armor("초급가죽갑옷", CodeType.BodyArmor, "armor");
		_armor.AddStatus(StatusConstant.HP, 5f);
		_armor.AddStatus(StatusConstant.DEF, 5f);
		_armor.SetGold(50);
		m_Equipment.Add(_armor);

		_armor = new Armor("초급가죽장갑", CodeType.Gloves, "gloves");
		_armor.AddStatus(StatusConstant.ATK, 5f);
		_armor.AddStatus(StatusConstant.DEF, 2f);
		_armor.SetGold(30);
		m_Equipment.Add(_armor);

		_armor = new Armor("초급가죽장화", CodeType.Boots, "boots");
		_armor.AddStatus(StatusConstant.DEF, 2f);
		_armor.SetGold(25);
		m_Equipment.Add(_armor);
	}
	//악세서리
	void AccSetting() {
		Acc _acc;
		_acc = new Acc("초급목걸이", CodeType.Amulet, "necklace");
		_acc.AddStatus(StatusConstant.HP, 10f);
		_acc.SetGold(100);
		m_Equipment.Add(_acc);

		_acc = new Acc("초급반지", CodeType.Ring, "rings");
		_acc.AddStatus(StatusConstant.HP, 10f);
		_acc.SetGold(100);
		m_Equipment.Add(_acc);

		_acc = new Acc("초급허리띠", CodeType.Belt, "belts");
		_acc.AddStatus(StatusConstant.HP, 10f);
		_acc.SetGold(100);
		m_Equipment.Add(_acc);
	}
	//무기
	void WeaponSetting() {
		Weapon _weapon;
		_weapon = new Weapon("초급활", CodeType.Weapon, "b_t_01");
		_weapon.AddStatus(StatusConstant.ATK, 10f);
		_weapon.AddStatus(StatusConstant.ATKSpeed, 1f);
		_weapon.SetGold(50);
		m_Equipment.Add(_weapon);
		
		_weapon = new Weapon("초급활살통", CodeType.SWeapon, "b_t_01");
		_weapon.AddStatus(StatusConstant.ATK, 5f);
		_weapon.SetGold(50);
		m_Equipment.Add(_weapon);
	}
	/// <summary>
	/// 게임테스트장비
	/// </summary>
	void TestSetting () {
		Acc _w = new Acc("테스트01 반지", CodeType.Ring, "rings");
		_w.AddStatus(StatusConstant.DEF, 5f);
		_w.AddStatus(StatusConstant.HP, 150f);
		_w.AddStatus(StatusConstant.MP, 50f);
		m_Equipment.Add(_w);

		_w = new Acc("테스트02 반지", CodeType.Ring, "rings");
		_w.AddStatus(StatusConstant.DEF, 5f);
		_w.AddStatus(StatusConstant.HP, 50f);
		_w.AddStatus(StatusConstant.MP, 150f);
		m_Equipment.Add(_w);

	}

	/// <summary>
	/// 장비 아이템 추가
	/// </summary>
	void EquipmentItem() {
		ArmorSetting();
		AccSetting();
		WeaponSetting();
		TestSetting();
	}

	//선택한 아이템 및 번호 리셋
	public static void IndesReset() {
		Inventory.selItem = null;
		EquipmentInventory.scItem = null;
		Inventory.Index = -1;
		EquipmentInventory.Index = -1;
	}

	public void DataBGActive(bool _is) {
		Inventory.ItemDataBG.gameObject.SetActive(_is);
		EquipmentInventory.EquipmentDataBG.gameObject.SetActive(_is);
	}

	/// <summary>
	/// 장비 장착
	/// </summary>
	public void SetEquipment(int _eodeE, Item _item) {
		cEquipment[_eodeE] = _item;
		if (_eodeE == CodeE.OnePortion || _eodeE == CodeE.TwoPortion) {

		} else {
			PlayerInformation.m_PlayerStats.AddEquipment(cEquipment[_eodeE].GetStaus());
		}
		GameMaster.Instance.Renew();
	}

	/// <summary>
	/// 로드시 장비 장착
	/// </summary>
	public void RenewalEquipment() {
		for (int i = 0 ; i < cEquipment.Length ; i++) {
			if (cEquipment[i] != null) {
				if ((int)cEquipment[i].GetCodeType() == CodeE.OnePortion || (int)cEquipment[i].GetCodeType() == CodeE.TwoPortion) {

				} else {
					PlayerInformation.m_PlayerStats.AddEquipment(cEquipment[i].GetStaus());
				}
			}
		}
		GameMaster.Instance.Renew();
	}

	/// <summary>
	/// 장비해제
	/// </summary>
	public void RemoveEquipment(int _eodeE, int _inventory) {
		if (_eodeE == CodeE.OnePortion || _eodeE == CodeE.TwoPortion) {
			
		} else {
			PlayerInformation.m_PlayerStats.RemoveEquipment(cEquipment[_eodeE].GetStaus());
		}
		
		cInventory[_inventory] = cEquipment[_eodeE];
		cEquipment[_eodeE] = null;
		GameMaster.Instance.Renew();
	}
	/// <summary>
	/// 인벤토에 아이템 추가
	/// </summary>
	/// <param name="_item">인벤토리에서 위치</param>
	public static int AddItem(Item _item) {
		int _num = System.Array.FindIndex(cInventory, a => a == null);
		if(_num != -1) cInventory[_num] = new Item(_item);
		return _num;
	}

	/// <summary>
	/// HP물약 먹을 때
	/// </summary>
	public void HPQuickslot() {
		if (PlayerInformation.m_PlayerStats.m_Status[StatusConstant.HP].GetChange() > PlayerInformation.m_PlayerStats.m_Status[StatusConstant.HP].GetPrint() &&
			cEquipment[CodeE.OnePortion] != null &&
			PlayerInformation.m_PlayerStats.m_Status[StatusConstant.HP].GetPrint() > 0) {
			if (cEquipment[CodeE.OnePortion].GetAmount() != 0) {
				PlayerInformation.m_PlayerStats.ReceiveData(StatusConstant.HP, cEquipment[CodeE.OnePortion].GetStaus(StatusConstant.HP));
				TextManager.Instance.SetText(PlayerInformation.m_PlayerController.transform.position, cEquipment[CodeE.OnePortion].GetStaus(StatusConstant.HP), new Color(0,1,0));
				cEquipment[CodeE.OnePortion].ItemUes();
			}
			if (cEquipment[CodeE.OnePortion].GetAmount() <= 0) {
				cEquipment[CodeE.OnePortion] = null;
				EquipmentInventory.Images[CodeE.OnePortion].sprite = spEquipmentNull;
			}
		}
	}

	/// <summary>
	/// MP물약 먹을 때
	/// </summary>
	public void MPQuickslot() {
		if (PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MP].GetChange() > PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MP].GetPrint() &&
			cEquipment[CodeE.TwoPortion] != null &&
			PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MP].GetPrint() > 0) {
			if (cEquipment[CodeE.TwoPortion].GetAmount() != 0) {
				PlayerInformation.m_PlayerStats.ReceiveData(StatusConstant.MP, cEquipment[CodeE.TwoPortion].GetStaus(StatusConstant.MP));
				TextManager.Instance.SetText(PlayerInformation.m_PlayerController.transform.position, cEquipment[CodeE.TwoPortion].GetStaus(StatusConstant.MP), new Color(0, 0, 1));
				cEquipment[CodeE.TwoPortion].ItemUes();
			}
			if (cEquipment[CodeE.TwoPortion].GetAmount() <= 0) {
				cEquipment[CodeE.TwoPortion] = null;
				EquipmentInventory.Images[CodeE.TwoPortion].sprite = spEquipmentNull;
			}
		}
	}

	/// <summary>
	/// 아이템 장착
	/// </summary>
	public void ItemFitting() {

		Item _d = cEquipment[(int)Inventory.selItem.GetCodeType()]; //장비하고 있다면 더미에 저장
		bool _isTwoRing = true;
		int _indes = EquipmentInventory.Index;

		if(_indes == 5 && cEquipment[CodeE.OneRing] != null && cEquipment[CodeE.TwoRing] == null) { // 아이템이 반지이고 두번째 반지 칸이 비여있다면 장착
			cEquipment[CodeE.TwoRing] = cInventory[Inventory.Index];
			_isTwoRing = false;
			_indes = CodeE.TwoRing;
		} else { //아이템 장착
			cEquipment[_indes] = cInventory[Inventory.Index];
			cInventory[Inventory.Index] = _d;
		}

		if (_indes == CodeE.OnePortion || _indes == CodeE.TwoPortion) {
			
		} else {
			PlayerInformation.m_PlayerStats.AddEquipment(cEquipment[_indes].GetStaus());
		}

		//아이템 UI 셋팅
		if(_isTwoRing && EquipmentInventory.scItem != null) {//해당 부위에 아이템이 있을 때
			if (_indes == CodeE.OnePortion || _indes == CodeE.TwoPortion) {

			} else {
				PlayerInformation.m_PlayerStats.RemoveEquipment(cEquipment[_indes].GetStaus());
			}
			EquipmentInventory.Images[_indes].sprite = cEquipment[_indes].GetImg();
			Inventory.ItemImages[Inventory.Index].sprite = cInventory[Inventory.Index].GetImg();
		} else {//해당 부위에 아이템이 없을 때
			EquipmentInventory.Images[_indes].sprite = cEquipment[_indes].GetImg();
			Inventory.ItemImages[Inventory.Index].sprite = spItemNull;
			cInventory[Inventory.Index] = null;
		}
		
		DataBGActive(false);
		IndesReset();
		GameMaster.Instance.Renew();
	}

	/// <summary>
	/// 장비해제
	/// </summary>
	public void EquipmentRelease() {
		int _num = -1;

		_num = System.Array.FindIndex(cInventory, a => a == null);
		if (_num != -1) {
			RemoveEquipment(EquipmentInventory.Index, _num);
			Inventory.ItemImages[_num].sprite = cInventory[_num].GetImg();

			EquipmentInventory.Images[EquipmentInventory.Index].sprite = spEquipmentNull;

			DataBGActive(false);
			IndesReset();
		}
		GameMaster.Instance.Renew();
	}

	/// <summary>
	/// 필드에 아이템 생성
	/// </summary>
	/// <param name="_itme">생성할 아이템</param>
	/// <param name="_pos">생성 위치</param>
	public static void FielditemInstantiate(Item _itme, Vector3 _pos) {
		Fielditem _fi = Instantiate(Instance.m_ItemObj, _pos, Quaternion.identity);
		_fi.Setting(_itme);
	}

	/// <summary>
	/// 인벤토리의 아이템 버리기
	/// </summary>
	public void InventoryItemDiscard() {
		Inventory.ItemImages[Inventory.Index].sprite = spItemNull;
		Inventory.ItemDataBG.gameObject.SetActive(false);

		Fielditem _fi = Instantiate(m_ItemObj, PlayerInformation.m_PlayerController.transform.position, Quaternion.identity);
		_fi.Setting(cInventory[Inventory.Index], false);
		cInventory[Inventory.Index] = null;

		DataBGActive(false);
		IndesReset();
	}

	/// <summary>
	/// 아이템 생성
	/// </summary>
	public void ItemProduce(Vector3 _pos) {
		Fielditem _fi = Instantiate(m_ItemObj, _pos, Quaternion.identity);
		int _num = Random.Range(0, m_Equipment.Count - 1);
		_fi.Setting(m_Equipment[_num], true);
	}

	/// <summary>
	/// 팔기
	/// </summary>
	public void ItemSell() {
		DataBGActive(false);
		IndesReset();
	}

	/// <summary>
	/// 장비 및 인벤토리 리셋
	/// </summary>
	/// <param name="_newCheck">Image Get 설정 </param>
	public static void ItemImgReset(bool _newCheck = false) {
		for(int i = 0; i < 30; i++) {
			if (_newCheck) { Inventory.ItemImages[i] = Inventory.cThis.GetChild(i).GetComponent<Image>(); }
			if(cInventory[i] != null) {
				Inventory.ItemImages[i].sprite = cInventory[i].GetImg();
			} else {
				Inventory.ItemImages[i].sprite = spItemNull;
			}
		}
		for(int i = 0; i < 12; i++) {
			if (_newCheck) { EquipmentInventory.Images[i] = EquipmentInventory.cThis.GetChild(i).GetComponent<Image>(); }
			if(cEquipment[i] != null) {
				EquipmentInventory.Images[i].sprite = cEquipment[i].GetImg();
			} else {
				EquipmentInventory.Images[i].sprite = spEquipmentNull;
			}
		}
	}
}