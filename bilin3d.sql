/*
Navicat MySQL Data Transfer

Source Server         : 139.196.19.138
Source Server Version : 50624
Source Host           : 139.196.19.138:3306
Source Database       : bilin3d

Target Server Type    : MYSQL
Target Server Version : 50624
File Encoding         : 65001

Date: 2015-10-26 08:49:40
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_address
-- ----------------------------
DROP TABLE IF EXISTS `t_address`;
CREATE TABLE `t_address` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) DEFAULT NULL COMMENT '会员ID',
  `Company` varchar(255) DEFAULT NULL COMMENT '公司名称',
  `Consignee` varchar(128) DEFAULT NULL COMMENT '收货人',
  `Tel` varchar(64) DEFAULT NULL COMMENT '手机号码',
  `Province` varchar(128) DEFAULT NULL COMMENT '省份',
  `City` varchar(128) DEFAULT NULL COMMENT '城市',
  `Dist` varchar(128) DEFAULT NULL COMMENT '县区',
  `Address` varchar(255) DEFAULT NULL COMMENT '详细地址',
  `State` int(3) DEFAULT '0' COMMENT '状态  0常规; 1默认',
  `EditTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8 COMMENT='收货地址';

-- ----------------------------
-- Records of t_address
-- ----------------------------
INSERT INTO `t_address` VALUES ('21', '3', 'aaaabbbbb', 'aaaaaaabbbbbbbbbbbbbbb', '111111111111', '湖北', '鄂州', '梁子湖区', '1111122222', '0', '2015-09-05 17:55:05');
INSERT INTO `t_address` VALUES ('22', '3', 'bbbbb', 'bbbbbb', 'bbbbbbbb', '福建', '厦门', '海沧区', 'bbbbbbbb', '1', '2015-09-05 17:55:49');
INSERT INTO `t_address` VALUES ('23', '3', 'cccccc', 'ccccc', 'cccccc', '辽宁', '鞍山', '立山区', 'ccccc', '0', '2015-09-05 17:55:49');
INSERT INTO `t_address` VALUES ('24', '18', '比邻三维科技', '林逢春', '13328557700', '福建', '泉州', '鲤城区', '德泰路51号4楼A区', '1', '2015-09-28 11:56:49');

-- ----------------------------
-- Table structure for t_article
-- ----------------------------
DROP TABLE IF EXISTS `t_article`;
CREATE TABLE `t_article` (
  `Id` int(11) NOT NULL,
  `Title` varchar(255) DEFAULT NULL COMMENT '标题',
  `Content` varchar(255) DEFAULT NULL COMMENT '内容',
  `TypeId` int(5) DEFAULT NULL COMMENT '类别ID(1.知识教育，2.帮忙中心，3.使用说明)',
  `State` int(3) DEFAULT NULL COMMENT '状态',
  `EditTime` timestamp NULL DEFAULT NULL,
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='知识教育，帮忙中心，使用说明';

-- ----------------------------
-- Records of t_article
-- ----------------------------

-- ----------------------------
-- Table structure for t_car
-- ----------------------------
DROP TABLE IF EXISTS `t_car`;
CREATE TABLE `t_car` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `UserId` int(11) DEFAULT NULL COMMENT '会员ID',
  `Amount` decimal(10,2) DEFAULT NULL COMMENT '购物车总金额',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `EditTime` timestamp NULL DEFAULT NULL COMMENT '编辑时间',
  `CarId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COMMENT='购物车主表';

-- ----------------------------
-- Records of t_car
-- ----------------------------
INSERT INTO `t_car` VALUES ('2', '3', '100.00', '2015-09-02 12:17:45', '2015-09-09 21:34:04', 'b5fb6bb49ca94fc5a71c43be1742c175');

-- ----------------------------
-- Table structure for t_cardetail
-- ----------------------------
DROP TABLE IF EXISTS `t_cardetail`;
CREATE TABLE `t_cardetail` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `CarId` varchar(255) DEFAULT NULL COMMENT '主表ID',
  `FileName` varchar(255) DEFAULT NULL COMMENT '3D文件名称',
  `Weight` varchar(255) DEFAULT NULL COMMENT '产品重量',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `EditTime` timestamp NULL DEFAULT NULL COMMENT '修改时间',
  `Area` varchar(255) DEFAULT NULL COMMENT '表面积',
  `Size` varchar(255) DEFAULT NULL COMMENT '尺寸',
  `Num` int(11) DEFAULT NULL COMMENT '份数',
  `MaterialId` int(11) DEFAULT NULL COMMENT '材料',
  `Price` decimal(10,2) DEFAULT NULL COMMENT ' 价格字段',
  `Amount` decimal(10,2) DEFAULT NULL COMMENT '金额',
  `Volume` float(10,2) DEFAULT NULL COMMENT '体积',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COMMENT='购物车子表';

-- ----------------------------
-- Records of t_cardetail
-- ----------------------------
INSERT INTO `t_cardetail` VALUES ('4', 'b5fb6bb49ca94fc5a71c43be1742c175', '3$2015-09-02-12-17-55-41958$立体拼图鹰.stl', '365.74', '2015-09-02 12:17:45', '2015-09-02 14:16:08', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '1', '1', '6.00', '100.00', '365.74');

-- ----------------------------
-- Table structure for t_cardetailtemp
-- ----------------------------
DROP TABLE IF EXISTS `t_cardetailtemp`;
CREATE TABLE `t_cardetailtemp` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `CarId` varchar(255) DEFAULT NULL COMMENT '主表ID',
  `FileName` varchar(255) DEFAULT NULL COMMENT '3D文件名称',
  `Weight` double(10,2) DEFAULT NULL COMMENT '产品重量',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `EditTime` timestamp NULL DEFAULT NULL COMMENT '修改时间',
  `Area` varchar(255) DEFAULT NULL COMMENT '表面积',
  `Size` varchar(255) DEFAULT NULL COMMENT '尺寸',
  `Num` int(11) DEFAULT NULL COMMENT '份数',
  `MaterialId` int(11) DEFAULT NULL COMMENT '材料',
  `Price` decimal(10,2) DEFAULT NULL COMMENT '金额',
  `Amount` decimal(10,2) DEFAULT NULL COMMENT '金额',
  `Volume` float(10,2) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COMMENT='未登陆时购物车子表';

-- ----------------------------
-- Records of t_cardetailtemp
-- ----------------------------
INSERT INTO `t_cardetailtemp` VALUES ('1', '69a77fbb7a4a4660b30c9710ee35849c', 'temp-f5b85b0d-3a01-454f-a2aa-2a0081d034db$2015-08-31-02-33-51-73768$立体拼图鹰.stl', '365.74', '2015-08-31 14:34:01', '2015-08-31 14:34:02', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '15', '1', '6.00', '100.00', '365.74');
INSERT INTO `t_cardetailtemp` VALUES ('2', '69a77fbb7a4a4660b30c9710ee35849c', 'temp-f5b85b0d-3a01-454f-a2aa-2a0081d034db$2015-08-31-02-33-51-73768$立体拼图鹰.stl', '365.74', '2015-08-31 14:34:02', '2015-08-31 14:34:02', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '5', '2', '8.00', '100.00', '365.74');
INSERT INTO `t_cardetailtemp` VALUES ('3', '1e67846939d34e23977c25e12d631443', 'temp-b24ec54e-707c-4d24-84dc-e08d6c8ea2ec$2015-08-31-02-37-18-05412$立体拼图鹰.stl', '365.74', '2015-08-31 14:37:18', '2015-08-31 14:37:24', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '2', '1', '6.00', '100.00', '365.74');
INSERT INTO `t_cardetailtemp` VALUES ('4', '1e67846939d34e23977c25e12d631443', 'temp-b24ec54e-707c-4d24-84dc-e08d6c8ea2ec$2015-08-31-02-37-18-05412$立体拼图鹰.stl', '365.74', '2015-08-31 14:37:19', '2015-08-31 14:37:24', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '2', '2', '8.00', '100.00', '365.74');
INSERT INTO `t_cardetailtemp` VALUES ('5', '1e67846939d34e23977c25e12d631443', 'temp-b24ec54e-707c-4d24-84dc-e08d6c8ea2ec$2015-08-31-02-37-18-05412$立体拼图鹰.stl', '365.74', '2015-08-31 14:37:19', '2015-08-31 14:37:24', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '2', '3', '12.00', '100.00', '365.74');
INSERT INTO `t_cardetailtemp` VALUES ('6', 'c1664b00c77541dcb1b4ff9facd70349', 'temp-33ea9c1e-ca28-4a6a-b75c-5d9f85e29db5$2015-10-23-02-09-37-56488$立体拼图鹰.stl', '365.74', '2015-10-23 14:09:42', '2015-10-23 14:09:42', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '3', '1', '6.00', '100.00', '365.74');

-- ----------------------------
-- Table structure for t_cartemp
-- ----------------------------
DROP TABLE IF EXISTS `t_cartemp`;
CREATE TABLE `t_cartemp` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `UserId` varchar(80) DEFAULT NULL COMMENT '会员临时ID',
  `Amount` decimal(10,2) DEFAULT NULL COMMENT '购物车总金额',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `EditTime` timestamp NULL DEFAULT NULL COMMENT '编辑时间',
  `CarId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COMMENT='未登陆时购物车主表';

-- ----------------------------
-- Records of t_cartemp
-- ----------------------------
INSERT INTO `t_cartemp` VALUES ('1', 'temp-f5b85b0d-3a01-454f-a2aa-2a0081d034db', '200.00', '2015-08-31 14:34:01', '2015-08-31 14:34:02', '69a77fbb7a4a4660b30c9710ee35849c');
INSERT INTO `t_cartemp` VALUES ('2', 'temp-b24ec54e-707c-4d24-84dc-e08d6c8ea2ec', '300.00', '2015-08-31 14:37:18', '2015-08-31 14:37:24', '1e67846939d34e23977c25e12d631443');
INSERT INTO `t_cartemp` VALUES ('3', 'temp-33ea9c1e-ca28-4a6a-b75c-5d9f85e29db5', '100.00', '2015-10-23 14:09:42', '2015-10-23 14:09:42', 'c1664b00c77541dcb1b4ff9facd70349');

-- ----------------------------
-- Table structure for t_charge
-- ----------------------------
DROP TABLE IF EXISTS `t_charge`;
CREATE TABLE `t_charge` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '自增长ID',
  `OrderId` int(11) DEFAULT NULL COMMENT '订单ID',
  `UserId` int(11) DEFAULT NULL COMMENT '会员ID',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '时间',
  `Amount` decimal(10,2) DEFAULT NULL COMMENT '支付金额',
  `PayFrom` varchar(255) DEFAULT NULL COMMENT '支付渠道',
  `Account` varchar(255) DEFAULT NULL COMMENT '支付帐号',
  `PayOrderId` varchar(255) DEFAULT NULL COMMENT '支付渠道订单编号',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='充值明细表';

-- ----------------------------
-- Records of t_charge
-- ----------------------------

-- ----------------------------
-- Table structure for t_config
-- ----------------------------
DROP TABLE IF EXISTS `t_config`;
CREATE TABLE `t_config` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `key` varchar(255) DEFAULT NULL,
  `value` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COMMENT='网站配置';

-- ----------------------------
-- Records of t_config
-- ----------------------------
INSERT INTO `t_config` VALUES ('1', '密码找回邮箱', '123@qq.com');
INSERT INTO `t_config` VALUES ('2', '密码找回邮箱模版', null);

-- ----------------------------
-- Table structure for t_material
-- ----------------------------
DROP TABLE IF EXISTS `t_material`;
CREATE TABLE `t_material` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `Name` varchar(255) DEFAULT NULL COMMENT '材料名称',
  `Color` varchar(255) DEFAULT NULL COMMENT '颜色',
  `Accuracy` varchar(255) DEFAULT NULL COMMENT '精度',
  `Density` float(10,2) DEFAULT NULL COMMENT '密度 (克/立方厘米)',
  `Delivery` varchar(255) DEFAULT NULL COMMENT '交货时间',
  `Price` decimal(10,2) DEFAULT NULL COMMENT '单价',
  `Price1` decimal(10,2) DEFAULT NULL COMMENT '单次最低价格',
  `State` int(255) DEFAULT '0' COMMENT '状态  0.正常、1.停用、2.删除',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COMMENT='材料单价表';

-- ----------------------------
-- Records of t_material
-- ----------------------------
INSERT INTO `t_material` VALUES ('1', '光敏树脂(普通料）', '白色 ', '0.1毫米', '1.00', '24小时', '6.00', '100.00', '0');
INSERT INTO `t_material` VALUES ('2', '光敏树脂（韧性料，类ABS)', '白色 ', '0.1毫米', '1.00', '24小时', '8.00', '100.00', '0');
INSERT INTO `t_material` VALUES ('3', '光敏树脂（透明料)', '透明', '0.1毫米', '1.00', '24小时', '12.00', '100.00', '0');
INSERT INTO `t_material` VALUES ('4', '光敏树脂(高精度)', '白色', '0.05毫米', '1.00', '24小时', '10.00', '200.00', '0');
INSERT INTO `t_material` VALUES ('5', '光敏树脂(超高精度)', '半透明', '0.016毫米\r\n', '1.00', '4天', '28.00', '300.00', '0');
INSERT INTO `t_material` VALUES ('6', '尼龙', '白色', '0.1毫米', '1.00', '5天', '15.00', '300.00', '0');
INSERT INTO `t_material` VALUES ('7', '玻纤', '浅灰色', '0.1毫米', '1.00', '5天', '15.00', '300.00', '0');
INSERT INTO `t_material` VALUES ('8', '不锈钢', '暗色', '0.1毫米', '8.00', '7天', '188.00', '2000.00', '0');
INSERT INTO `t_material` VALUES ('9', '钛合金', '暗色', '0.1毫米', '5.00', '7天', '188.00', '2000.00', '0');

-- ----------------------------
-- Table structure for t_order
-- ----------------------------
DROP TABLE IF EXISTS `t_order`;
CREATE TABLE `t_order` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `OrderId` varchar(255) DEFAULT NULL COMMENT '订单号',
  `UserId` int(11) DEFAULT NULL COMMENT '会员ID',
  `StateId` int(11) DEFAULT NULL,
  `Amount` decimal(10,2) DEFAULT NULL COMMENT '购物车总金额',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `EditTime` timestamp NULL DEFAULT NULL COMMENT '编辑时间',
  `AddressId` int(11) DEFAULT NULL COMMENT '收货地址',
  `Remark` varchar(255) DEFAULT NULL COMMENT '订单备注',
  `Express` varchar(255) DEFAULT NULL COMMENT '快递信息',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='购物车主表';

-- ----------------------------
-- Records of t_order
-- ----------------------------
INSERT INTO `t_order` VALUES ('1', '3201508310238170474', '3', '1', '366.00', '2015-08-31 14:38:13', '2015-08-31 14:38:13', '2', '', '送货方式：普通快递 承运人：京东快递货运单号：9645442678');

-- ----------------------------
-- Table structure for t_orderdetail
-- ----------------------------
DROP TABLE IF EXISTS `t_orderdetail`;
CREATE TABLE `t_orderdetail` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `OrderId` varchar(255) DEFAULT NULL COMMENT '主表ID',
  `FileName` varchar(255) DEFAULT NULL COMMENT '3D文件名称',
  `Weight` double(10,2) DEFAULT NULL COMMENT '产品重量',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `EditTime` timestamp NULL DEFAULT NULL COMMENT '修改时间',
  `Volume` float(10,2) DEFAULT '0.00',
  `Area` varchar(255) DEFAULT NULL COMMENT '表面积',
  `Size` varchar(255) DEFAULT NULL COMMENT '尺寸',
  `Num` int(11) DEFAULT NULL COMMENT '份数',
  `MaterialId` int(11) DEFAULT NULL COMMENT '材料',
  `Price` decimal(10,2) DEFAULT NULL COMMENT ' 价格字段',
  `Amount` decimal(10,2) DEFAULT '0.00' COMMENT '金额',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COMMENT='购物车子表';

-- ----------------------------
-- Records of t_orderdetail
-- ----------------------------
INSERT INTO `t_orderdetail` VALUES ('1', '3201508310238170474', '3$2015-08-31-02-37-18-05412$立体拼图鹰.stl', '365.74', '2015-08-31 14:38:13', '2015-08-31 14:38:13', '365.74', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '2', '1', '6.00', '100.00');
INSERT INTO `t_orderdetail` VALUES ('2', '3201508310238170474', '3$2015-08-31-02-37-18-05412$立体拼图鹰.stl', '365.74', '2015-08-31 14:38:13', '2015-08-31 14:38:13', '365.74', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '2', '2', '8.00', '100.00');
INSERT INTO `t_orderdetail` VALUES ('3', '3201508310238170474', '3$2015-08-31-02-37-18-05412$立体拼图鹰.stl', '365.74', '2015-08-31 14:38:13', '2015-08-31 14:38:13', '365.74', '663.9  mm²', '9.85 * 34.72 * 1.35  mm', '12', '3', '12.00', '144.00');

-- ----------------------------
-- Table structure for t_orderstate
-- ----------------------------
DROP TABLE IF EXISTS `t_orderstate`;
CREATE TABLE `t_orderstate` (
  `Id` int(11) NOT NULL,
  `statename` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of t_orderstate
-- ----------------------------
INSERT INTO `t_orderstate` VALUES ('1', '已下单-待付款');
INSERT INTO `t_orderstate` VALUES ('2', '已付款-待管理员审核');
INSERT INTO `t_orderstate` VALUES ('3', '审核未通过-待退款');
INSERT INTO `t_orderstate` VALUES ('4', '已退款');
INSERT INTO `t_orderstate` VALUES ('5', '审核成功-已安排生产');
INSERT INTO `t_orderstate` VALUES ('6', '已发货');
INSERT INTO `t_orderstate` VALUES ('7', '已收货');
INSERT INTO `t_orderstate` VALUES ('8', '关闭、完成');

-- ----------------------------
-- Table structure for t_pay
-- ----------------------------
DROP TABLE IF EXISTS `t_pay`;
CREATE TABLE `t_pay` (
  `Id` int(11) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of t_pay
-- ----------------------------
INSERT INTO `t_pay` VALUES ('1', '支付宝');
INSERT INTO `t_pay` VALUES ('2', '微信支付');
INSERT INTO `t_pay` VALUES ('3', '建行卡');

-- ----------------------------
-- Table structure for t_payment
-- ----------------------------
DROP TABLE IF EXISTS `t_payment`;
CREATE TABLE `t_payment` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '自增长ID',
  `OrderId` int(11) DEFAULT NULL COMMENT '订单ID',
  `UserId` int(11) DEFAULT NULL COMMENT '会员ID',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '时间',
  `Amount` decimal(10,2) DEFAULT NULL COMMENT '支付金额',
  `PayFrom` varchar(255) DEFAULT NULL COMMENT '支付渠道',
  `Account` varchar(255) DEFAULT NULL COMMENT '支付帐号',
  `PayOrderId` varchar(255) DEFAULT NULL COMMENT '支付渠道订单编号',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='支付明细表';

-- ----------------------------
-- Records of t_payment
-- ----------------------------

-- ----------------------------
-- Table structure for t_user
-- ----------------------------
DROP TABLE IF EXISTS `t_user`;
CREATE TABLE `t_user` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID自增',
  `UserGuid` varchar(48) NOT NULL COMMENT 'guid用户验证时用',
  `Email` varchar(255) DEFAULT NULL COMMENT '电子邮箱',
  `NickName` varchar(255) DEFAULT NULL COMMENT '会员昵称',
  `Tel` varchar(255) DEFAULT NULL COMMENT '联系电话',
  `Avatars` varchar(255) DEFAULT NULL COMMENT '会员头像',
  `PassWord` varchar(255) DEFAULT NULL COMMENT '会员密码',
  `PointTotal` int(255) DEFAULT '0' COMMENT '积分总额',
  `PointRemain` int(255) DEFAULT '0' COMMENT '积分余额',
  `Expense` decimal(10,2) DEFAULT '0.00' COMMENT '消费总额',
  `Balance` decimal(10,2) DEFAULT '0.00' COMMENT '账户余额',
  `State` int(3) DEFAULT '0' COMMENT '状态  0.正常、1.停用、2.删除',
  `EditTime` timestamp NULL DEFAULT NULL COMMENT '编辑时间',
  `CreateTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  KEY `IX_UserId` (`UserGuid`),
  KEY `IX_Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8 COMMENT='用户(会员)表';

-- ----------------------------
-- Records of t_user
-- ----------------------------
INSERT INTO `t_user` VALUES ('2', '2b16054f-67e3-49ca-bff0-fb0357bafaee', 'ayumi043@163.com', null, null, null, '111111', '0', '0', '0.00', null, '0', null, null);
INSERT INTO `t_user` VALUES ('3', '406de908-0eab-4be4-bfc4-5fcab39f4853', '123@abc.com', 'hello world!', '123232323', '3$2015-09-01-07-26-44-79365$傲游截图20150601150844.png', '111111', '0', '0', '0.00', null, '0', '2015-09-01 19:27:49', null);
INSERT INTO `t_user` VALUES ('12', 'cd0df897-f1fe-463c-a123-edf48b1e658e', 'test@test.com', null, null, null, '123456', '0', '0', '0.00', null, '0', null, '2015-07-21 14:17:08');
INSERT INTO `t_user` VALUES ('13', '7d287dd6-7b78-4420-b061-0dbe3b2ba9fe', '789@abc.com', null, null, null, '123456', '0', '0', '0.00', null, '0', null, '2015-08-01 14:33:03');
INSERT INTO `t_user` VALUES ('14', 'f12429f4-f696-40c8-be63-da7f22e4adf7', '999@999.com', null, null, null, '111111', '0', '0', '0.00', null, '0', null, '2015-08-03 18:46:02');
INSERT INTO `t_user` VALUES ('15', '14c3935f-732f-4b71-bec9-bf35e37dcc59', '111@111.com', null, null, null, '111111', '0', '0', '0.00', null, '0', null, '2015-08-03 19:05:54');
INSERT INTO `t_user` VALUES ('16', '6f0f1d72-e450-42c8-99fa-340c2508196d', '12345@12345.com', null, null, null, '111111', '0', '0', '0.00', null, '0', null, '2015-08-07 01:36:11');
INSERT INTO `t_user` VALUES ('17', '32cc08be-d8bb-49a8-b987-a35b164f9132', '111111@111111.com', null, null, null, '111111', '0', '0', '0.00', null, '0', null, '2015-08-10 21:06:53');
INSERT INTO `t_user` VALUES ('18', '1c6df431-6248-4217-8b8f-0933486050e6', '75579948@qq.com', null, null, null, '19821217lfc', '0', '0', '0.00', null, '0', null, '2015-08-31 11:16:21');
