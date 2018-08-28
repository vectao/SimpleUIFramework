--=======================================================
--  该文件为自动生成，请勿手动修改;
--  LastUpdate Time: 8/28/2018 9:08:24 PM
--=========================================================


UITestView = class("UITestView");

function UITestView:ctor(go, ...)
    self.name = "ui_test";
    self.gameObject = go;
	self.objectContainer = go:GetComponent("ObjectContainer");

    self.m_equipinfo = self.objectContainer:GetObjComponent("m_equipinfo");
    self.m_equipinfo_icon = self.objectContainer:GetObjComponent("m_equipinfo_icon");
    self.m_equip_buy = self.objectContainer:GetObjComponent("m_equip_buy");
    self.m_equip_price = self.objectContainer:GetObjComponent("m_equip_price");
    self.m_equip_back = self.objectContainer:GetObjComponent("m_equip_back");
    self.m_equipinfo_name_lbl = self.objectContainer:GetObjComponent("m_equipinfo_name_lbl");
    self.m_equipinfo_attr_lbl1 = self.objectContainer:GetObjComponent("m_equipinfo_attr_lbl1");
    self.m_equipinfo_attr_lbl2 = self.objectContainer:GetObjComponent("m_equipinfo_attr_lbl2");
    self.m_equipinfo_attr_lbl3 = self.objectContainer:GetObjComponent("m_equipinfo_attr_lbl3");
    self.m_equipinfo_attr_lbl4 = self.objectContainer:GetObjComponent("m_equipinfo_attr_lbl4");
    self.m_equipinfo_attr_txt_lbl1 = self.objectContainer:GetObjComponent("m_equipinfo_attr_txt_lbl1");
    self.m_equipinfo_attr_txt_lbl2 = self.objectContainer:GetObjComponent("m_equipinfo_attr_txt_lbl2");
    self.m_equipinfo_attr_txt_lbl3 = self.objectContainer:GetObjComponent("m_equipinfo_attr_txt_lbl3");
    self.m_equipinfo_attr_txt_lbl4 = self.objectContainer:GetObjComponent("m_equipinfo_attr_txt_lbl4");
    self.m_equip_grid = self.objectContainer:GetObjComponent("m_equip_grid");
    self.i_equip_item = self.objectContainer:GetObjComponent("i_equip_item");
    self.m_close = self.objectContainer:GetObjComponent("m_close");
    self.m_rod = self.objectContainer:GetObjComponent("m_rod");
    self.m_boat = self.objectContainer:GetObjComponent("m_boat");
    self.m_avatar = self.objectContainer:GetObjComponent("m_avatar");

    self.controller = UITestController.New(self, ...);
end

function UITestView:Awake()
    self.controller:Awake();
end

function UITestView:Start()
    self.controller:Start();
end

function UITestView:OnEnable()
    self.controller:OnEnable();
end

function UITestView:OnDisable()
    self.controller:OnDisable();
end

function UITestView:PreDestroy()
    self.controller:PreDestroy();
end

function UITestView:OnFocus(focus)
    self.controller:OnFocus(focus);
end

function UITestView:OnDestroy()
    self.controller:OnDestroy();
    self.name = nil;
    self.gameObject = nil;
    self.objectContainer = nil;
    self.controller = nil;
end

function UITestView:GetController()
    return self.controller;
end


UITestView.ItemEquipItemView = class("UITestView.ItemEquipItemView");

function UITestView.ItemEquipItemView:ctor(index, go)
	self.name = "i_equip_item";
	self.gameObject = go;
	self.objectContainer = go:GetComponent("ObjectContainer");

	self.m_img1 = self.objectContainer:GetObjComponent("m_img1");
	self.m_img2 = self.objectContainer:GetObjComponent("m_img2");
	self.m_lbl1 = self.objectContainer:GetObjComponent("m_lbl1");
	self.m_lbl2 = self.objectContainer:GetObjComponent("m_lbl2");

	self.controller = UITestController.ItemEquipItemController.New(index, self);
end

function UITestView.ItemEquipItemView:OnUpdateItem(index, go)
	self.controller:OnUpdateItem(index, go);
end

