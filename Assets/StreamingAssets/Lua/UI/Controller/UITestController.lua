--=======================================================
--  Author: 
--  Create Time: 8/28/2018 9:08:24 PM
--  Describe:
--=========================================================
local window = nil;
local Tabs = { rod = 1, boat = 2, skin = 3 }
local mEquipDataList = {};
local mEquipItemListGo = {};
local mEquipSelectedIndex = 1;

UITestController = class("UITestController");

function UITestController:ctor(view, arr)
    self.view = view;
    self:RegistEvent();
	
	window = self;
	self:Init();
end

function UITestController:RegistEvent()
	util.addListener(self, self.view.m_equip_buy:GetComponent("ButtonEx").onClick, self.BtnEquipBuyClick);
	util.addListener(self, self.view.m_equip_back:GetComponent("ButtonEx").onClick, self.BtnEquipBackClick);
	util.addListener(self, self.view.m_close:GetComponent("ButtonEx").onClick, self.BtnCloseClick);
	util.addListener(self, self.view.m_rod:GetComponent("ButtonEx").onClick, self.BtnRodClick);
	util.addListener(self, self.view.m_boat:GetComponent("ButtonEx").onClick, self.BtnBoatClick);
	util.addListener(self, self.view.m_avatar:GetComponent("ButtonEx").onClick, self.BtnAvatarClick);
end

function UITestController:UnRegistEvent()
	util.clearListener(self);
end

function UITestController:Awake()
	CS.UnityEngine.Debug.LogError("Awake")
end

function UITestController:Start()
	CS.UnityEngine.Debug.LogError("Start")
end

function UITestController:OnEnable()
	CS.UnityEngine.Debug.LogError("OnEnable")
end

function UITestController:OnDisable()
	CS.UnityEngine.Debug.LogError("OnDisable")
end

function UITestController:PreDestroy()
	CS.UnityEngine.Debug.LogError("PreDestroy")
	self.isDestroy = true;
	self:UnRegistEvent();
end

function UITestController:OnFocus(focus)
	CS.UnityEngine.Debug.LogError("OnFocus")
end

function UITestController:OnDestroy()
	CS.UnityEngine.Debug.LogError("OnDestroy")
end

function UITestController:BtnEquipBuyClick()
    CS.UnityEngine.Debug.LogError("购买成功");
end

function UITestController:Init()
    self.mTab = Tabs.rod;

    mEquipDataList = {};
    mEquipItemListGo = {};

    local scrollview = self.view.m_equip_grid:GetComponent("SimpleLoopScrollRect");
    scrollview.m_TotalCount = 3;
    scrollview.OnAddLuaItem = function(index, trans)
		return UITestController.ItemEquipItemController.New(index, trans); 
	end;
    scrollview:FillData();
end

function UITestController:ShowEquipList()
    self.view.m_equipinfo.gameObject:SetActive(false);
    self.view.m_equip_grid.gameObject:SetActive(true);
end

function UITestController:ShowEquipInfo(data)
    self.view.m_equipinfo.gameObject:SetActive(true);
    self.view.m_equip_grid.gameObject:SetActive(false);

    self.view.m_equipinfo_attr_lbl1:GetComponent("Text").text = "收线速度";
    self.view.m_equipinfo_attr_lbl2:GetComponent("Text").text = "甩杆力量";
    self.view.m_equipinfo_attr_lbl3:GetComponent("Text").text = "鱼线长度";
    self.view.m_equipinfo_attr_lbl4:GetComponent("Text").text = "鱼线强度";
end

function UITestController:HideEquipPage()
    self.view.m_equipinfo.gameObject:SetActive(false);
    self.view.m_equip_grid.gameObject:SetActive(false);
end

function UITestController:ShowBoatPage()
    self:HideEquipPage();
end

function UITestController:ShowSkinPage()
    self:HideEquipPage();
end

function UITestController:TabClilckHandler(tab)
    if(self.mTab == Tabs.boat) then
        self:ShowBoatPage();
    elseif(self.mTab == Tabs.skin) then
        self:ShowSkinPage();
    else
        self:ShowEquipList();
    end
end

function UITestController:BtnEquipBackClick()
    self:ShowEquipList();
end

function UITestController:BtnCloseClick()
    UIManager:CloseWindow(self.view.name);
end

function UITestController:BtnRodClick()
    self.mTab = Tabs.rod;
    self:TabClilckHandler(self.mTab);
end

function UITestController:BtnBoatClick()
    self.mTab = Tabs.boat;
    self:TabClilckHandler(self.mTab);
end

function UITestController:BtnAvatarClick()
    self.mTab = Tabs.skin;
    self:TabClilckHandler(self.mTab);
end




UITestController.ItemEquipItemController = class("UITestController.ItemEquipItemController");

function UITestController.ItemEquipItemController:ctor(index, view)
	self.view = view;
	self:Init();
	util.addListener(self, self.view.gameObject.transform:GetComponent("ButtonEx").onClick, self.OnClick);
end

function UITestController.ItemEquipItemController:Init()
	-- initialize ...
end

function UITestController.ItemEquipItemController:OnUpdateItem(index, go)
	-- OnUpdateItem ...
end

function UITestController.ItemEquipItemController:OnClick()
    window:ShowEquipInfo();
end