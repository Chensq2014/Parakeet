namespace Parakeet.Net.ServiceGroup.JianWei.HttpDtos
{
    public class ProjectWorkerDto
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 省级ID
        /// </summary>
        public string RootId { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public string WorkerId { get; set; }

        /// <summary>
        /// 人员名称
        /// </summary>
        public string WorkerName { get; set; }

        /// <summary>
        /// 身份证(掩码显示)
        /// </summary>
        public string WorkerIdNo { get; set; }

        /// <summary>
        /// 身份证sha1加密
        /// </summary>
        public string WorkerIdNoCoded { get; set; }

        /// <summary>
        /// 性别：MALEL：男FEMALE：女
        /// </summary>
        public string WorkerGender { get; set; }

        /// <summary>
        /// 头像图片URL
        /// </summary>
        public string WorkerHeaderImage { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string WorkerNation { get; set; }

        /// <summary>
        /// 所属企业
        /// </summary>
        public Corp Corp { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public WorkerType WorkerType { get; set; }

        /// <summary>
        /// 人员类别：M：管理人员W：作业工人
        /// </summary>
        public string WorkerCategory { get; set; }

        /// <summary>
        /// 人员状态：Entry：在职Exit：离职Locked：禁入
        /// </summary>
        public string EntryStatus { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public long? JoinDate { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        public long? LeaveDate { get; set; }

        /// <summary>
        /// 考勤方式：
        /// Face：人脸识别
        /// Eye：虹膜识别
        /// Finger：指纹识别
        /// Hand：掌纹识别
        /// IDCard：身份证识别
        /// RnCard：实名卡
        /// Error：异常清退
        /// Manuel：一键开闸
        /// ExitChannel：应急通道
        /// QRCode：二维码识别
        /// App：APP考勤
        /// Other：其他方式
        /// </summary>
        public string Mode { get; set; }

        public string CardNumber { get; set; }

        /// <summary>
        /// 考勤卡
        /// </summary>
        public AttendanceCard AttendanceCard { get; set; }

        /// <summary>
        /// 劳务公司
        /// </summary>
        public LaborCompany LaborCompany { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public long? UpdatedAt { get; set; }

        /// <summary>
        /// 是否班组长
        /// </summary>
        public bool? GroupLeader { get; set; }

        public WorkPost WorkPost { get; set; }

        public WorkerGroup WorkerGroup { get; set; }
    }

    public class Corp
    {
        /// <summary>
        /// 班组ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 班组编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        public string Name { get; set; }
    }

    public class WorkerType
    {
        /// <summary>
        /// 工种ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工种编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 工种名称
        /// </summary>
        public string Name { get; set; }
    }

    public class WorkPost
    {
        /// <summary>
        /// 岗位ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name { get; set; }
    }

    public class WorkerGroup
    {
        /// <summary>
        /// 班组ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 班组编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        public string Name { get; set; }
    }

    public class AttendanceCard
    {
        /// <summary>
        /// 考勤卡ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 考勤卡号
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// 考勤卡类别
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// 制卡时间
        /// </summary>
        public long? IssueCardDate { get; set; }

        public string IssueCardPic { get; set; }
    }

    public class LaborCompany
    {
        /// <summary>
        /// 劳务公司ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 劳务公司编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 劳务公司名称
        /// </summary>
        public string Name { get; set; }
    }
}
