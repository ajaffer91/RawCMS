import { optionalChain } from "/app/utils/object.utils.js";
import { SimplePieChart } from "/app/common/shared/components/charts/simple-pie-chart/simple-pie-chart.js";
import { dashboardService } from "/app/modules/core/services/dashboard.service.js";

const _DashboardDef = async () => {
    const tpl = await RawCMS.loadComponentTpl(
        "/app/modules/core/components/dashboard/dashboard.tpl.html"
    );

    return {
        components: {
            PieChart: SimplePieChart
        },
        computed: {
            totalRecordsNum: function () {
                const quotasObj = optionalChain(() => this.info.recordQuotas);
                if (quotasObj === undefined) {
                    return 0;
                }

                return Object.keys(quotasObj)
                    .map(x => quotasObj[x])
                    .reduce((acc, v) => acc + v, 0);
            },
            recordQuotasChartData: function () {
                const quotasObj = optionalChain(() => this.info.recordQuotas, {
                    fallbackValue: {}
                });
                const labels = [];
                const data = [];
                for (const index in quotasObj) {
                    labels.push(index);
                    data.push(quotasObj[index]);
                }

                return { data, labels };
            }
        },
        created: async function () {
            this.info = await this.dashboardService.getDashboardInfo();
            this.isLoading = false;
        },
        data: function () {
            return {
                chartOptions: {
                    lowerIsBetter: true
                },
                dashboardService: dashboardService,
                isLoading: true,
                info: undefined,
                optionalChain: optionalChain
            };
        },
        template: tpl
    };
};

const _Dashboard = async (res, rej) => {
    const cmpDef = await _DashboardDef();
    res(cmpDef);
};

export const DashboardDef = _DashboardDef;
export const Dashboard = _Dashboard;
export default _Dashboard;