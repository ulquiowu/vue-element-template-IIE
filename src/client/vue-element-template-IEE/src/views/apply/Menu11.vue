<template>
  <div>
    <!-- 面包屑导航区域 -->
    <el-breadcrumb>
      <el-breadcrumb-item :to="{ path: '/home' }">首页</el-breadcrumb-item>
      <el-breadcrumb-item>请购单</el-breadcrumb-item>
      <el-breadcrumb-item>1-1</el-breadcrumb-item>
    </el-breadcrumb>
    <!-- 查询区域 -->
    <el-form :inline="true" :rules="applyFormRules" :model="queryInfo" ref="applyFormRef">
      <el-form-item label="开始日期" prop="begindate">
        <el-date-picker
          type="date"
          placeholder="选择日期"
          v-model="queryInfo.begindate"
          value-format="yyyy-MM-dd"
        ></el-date-picker>
      </el-form-item>
      <el-form-item label="结束日期" prop="enddate">
        <el-date-picker
          type="date"
          placeholder="选择日期"
          v-model="queryInfo.enddate"
          value-format="yyyy-MM-dd"
        ></el-date-picker>
      </el-form-item>
      <el-form-item label="项目号">
        <el-input
          placeholder="项目号"
          v-model="queryInfo.projectcode"
        ></el-input>
      </el-form-item>
      <el-form-item label="物料编码">
        <el-input
          placeholder="物料编码"
          v-model="queryInfo.prodcode"
        ></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="getApplyDetailList(1)">查询</el-button>
      </el-form-item>
    </el-form>
    <!-- 批量删除按钮 -->
    <el-button type="danger" icon="el-icon-delete" size="medium" @click="batchDelete()" style="float:right;margin-bottom:10px">批量删除</el-button>
        <!-- 用户列表区域 -->
    <el-table :data="applydetaillist" border stripe ref='tableRef'>
      <el-table-column type="index" label='#' align="center"></el-table-column>
      <el-table-column type="selection" align="center"></el-table-column>
      <el-table-column label="请购单号" prop="billNum"></el-table-column>
      <el-table-column label="请购单项" prop="item"></el-table-column>
      <el-table-column label="物料编码" prop="prodCode"></el-table-column>
      <el-table-column label="物料名称" prop="prodName"></el-table-column>
      <el-table-column label="核准数" prop="allowQty"></el-table-column>
      <el-table-column label="已转采购数" prop="finQty"></el-table-column>
      <el-table-column label="项目号" prop="projectCode"></el-table-column>
      <el-table-column label="操作" width="120px" align="center" fixed="right">
        <template slot-scope="scope">
          <!-- 取消按钮 -->
          <el-tooltip effect="dark" content="删除请购信息" placement="top" :enterable="false">
            <el-button type="danger" icon="el-icon-delete" size="medium" @click="deleteApplyDetail(scope.row)">删除</el-button>
          </el-tooltip>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页区域 -->
    <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="queryInfo.pagenum" :page-sizes="[5, 10, 20, 50]" :page-size="queryInfo.pagesize" layout="total, sizes, prev, pager, next, jumper" :total="total">
    </el-pagination>
  </div>
</template>

<script>
export default {
  data () {
    return {
      // 获取用户列表的参数对象
      queryInfo: {
        // 当前的页数
        pagenum: 1,
        // 当前每页显示多少条数据
        pagesize: 5,
        // 厂区编码
        plant: '',
        // 开始时间
        begindate: '',
        // 结束时间
        enddate: '',
        // 项目号
        projectcode: '',
        // 物料编码
        prodcode: ''
      },
      applydetaillist: [],
      total: 0,
      applyFormRules: {
        begindate: [
          { required: true, message: '请选择开始日期', trigger: 'change' }
        ],
        enddate: [
          { required: true, message: '请选择结束日期', trigger: 'change' }
        ]
      }
    }
  },
  created () {
    this.queryInfo.plant = window.sessionStorage.getItem('plant')
  },
  methods: {
    async getApplyDetailList (currentpage) {
      this.$refs.applyFormRef.validate(async valid => {
        if (!valid) return
        this.applydetaillist = null
        this.queryInfo.pagenum = currentpage
        const { data: res } = await this.$http.get(
          `PreApply/${this.queryInfo.plant}/${this.queryInfo.pagenum}/${this.queryInfo.pagesize}/${this.queryInfo.begindate}/${this.queryInfo.enddate}`,
          {
            params: this.queryInfo
          }
        )
        console.log(res)
        if (res.meta.status !== 200) {
          return this.$message.error('没有获取到数据')
        }
        this.applydetaillist = res.details
        this.total = res.total
      })
    },
    // 监听 pagesize 改变事件
    handleSizeChange (newSize) {
      this.queryInfo.pagesize = newSize
      // pagesize 改变后默认跳转到第一页
      this.queryInfo.pagenum = 1
      this.getApplyDetailList(1)
    },
    // 监听 pagenum 改变事件
    handleCurrentChange (newPage) {
      this.queryInfo.pagenum = newPage
      this.getApplyDetailList(newPage)
    },
    // 删除请购单明细事件
    async deleteApplyDetail (applyDetailInfo) {
      // 询问是否删除
      const confirmResult = await this.$confirm('此操作将删除该请购信息, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).catch(err => {
        return err
      })
      // 获取用户确认类型
      if (confirmResult !== 'confirm') {
        return this.$message.error('已取消删除')
      }
      // 调用API删除请购单信息
      const { data: res } = await this.$http.put(`PreApply/${this.queryInfo.plant}/${applyDetailInfo.billNum}/${applyDetailInfo.item}`)
      if (res.meta.status !== 200) {
        return this.$message.error(`删除失败:${applyDetailInfo.billNum}-${applyDetailInfo.item}`)
      }
      this.$message.success(`删除成功:${applyDetailInfo.billNum}-${applyDetailInfo.item}`)
      // 删除成功后默认跳转到第一页
      this.getApplyDetailList(1)
    },
    // 批量删除事件
    async batchDelete () {
      // 获取选中行
      const selectRows = this.$refs.tableRef.selection
      // 判断是否选中信息
      if (selectRows.length === 0) {
        return this.$message.error('请选择需要删除的请购信息')
      }
      // 询问是否删除
      const confirmResult = await this.$confirm('此操作将批量删除已选择的请购信息, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).catch(err => {
        return err
      })
      // 获取用户确认类型
      if (confirmResult !== 'confirm') {
        return this.$message.error('已取消批量删除操作')
      }
      // 循环选中的信息
      const promises = selectRows.map(async (item, index) =>
        this.$http.put(`PreApply/${this.queryInfo.plant}/${item.billNum}/${item.item}`).then((res) => {
          if (res.data.meta.status !== 200) {
            return this.$message.error(`删除失败:${item.billNum}-${item.item}`)
          }
          this.$message.success(`删除成功:${item.billNum}-${item.item}`)
        })
      )
      await Promise.all(promises)
      // 操作结束后跳转到第一页
      this.getApplyDetailList(1)
    }
  }
}
</script>

<style lang="less" scoped>
</style>
