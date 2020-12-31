export class Chart {
    backgroundColor: string[];
    datasets: Dataset[];
    constructor() {
        this.datasets = [];
        this.datasets.push(new Dataset());
        this.backgroundColor = ["#05a4b5",'#327eaa', '#008380', '#418040', '#605e5c'];
    }
}
export class PieChart extends Chart {
    labels: string[];
    values: number[];
    type: string;
    colors:any;
    constructor() {
        super();
        this.labels = [];
        this.values = [];
        this.type = "pie";
        this.colors = [
            {
              backgroundColor:this.backgroundColor
            }];
    }
    donutColors:any;
}
export class Dataset {
    data: number[];
    backgroundColor: string[];
    constructor() {
        this.data = [];
        this.backgroundColor = ['#327eaa', '#d13438', '#418040', '#605e5c'];
    }
}