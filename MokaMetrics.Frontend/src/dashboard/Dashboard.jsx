
import { useState } from "react";
import "leaflet/dist/leaflet.css";
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip as ChartTooltip,
  ResponsiveContainer
} from "recharts";
import {
  MapContainer,
  TileLayer,
  CircleMarker,
  Tooltip as MapTooltip
} from "react-leaflet";

import { motion } from "framer-motion";
import { TrendingUp, Package, Layers, Bell } from "lucide-react";


// ──────────────────────────────────────────────────────────────────────────────
//  MOCK DATA (sostituisci con chiamate API reali)
// ──────────────────────────────────────────────────────────────────────────────
const kpi = {
  output: 136,
  oee: 92,
  lots: 5,
  alarms: 2
};

const production = [
  { time: "00:00", value: 12 },
  { time: "02:00", value: 20 },
  { time: "04:00", value: 25 },
  { time: "06:00", value: 30 },
  { time: "08:00", value: 40 },
  { time: "10:00", value: 50 },
  { time: "12:00", value: 58 },
  { time: "14:00", value: 60 },
  { time: "16:00", value: 70 }
];

const plants = [
  { name: "Italy", position: [45.43, 11.88], running: 10, idle: 2, alarm: 1 },
  { name: "Brazil", position: [-23.55, -46.63], running: 7, idle: 4, alarm: 0 },
  { name: "Vietnam", position: [10.82, 106.63], running: 9, idle: 3, alarm: 2 }
];

const events = [
  {
    severity: "Warning",
    time: "12:56",
    plant: "Italy",
    message: "Vibration threshold exceeded"
  },
  {
    severity: "Error",
    time: "16:06",
    plant: "Brazil",
    message: "Cooling system issue – Line 3"
  },
  {
    severity: "Info",
    time: "14:45",
    plant: "Vietnam",
    message: "Temperature back to nominal"
  }
];

const factory =
  [
    {
      name: "Italy Factory",
      machines: [
        {
          name: "Macchina CNC2",
          status: "error"
        },
        {
          name: "Macchina CNC3",
          status: "error"
        },
        {
          name: "Macchina CNC4",
          status: "error"
        }
      ]
    },
    {
      name: "Vietnam Factory",
      machines: [
        {
          name: "Macchina CNC2",
          status: "running"
        },
        {
          name: "Macchina CNC3",
          status: "idle"
        },
        {
          name: "Macchina CNC4",
          status: "running"
        }
      ]
    },
    {
      name: "Brasil Factory",
      machines: [
        {
          name: "Macchina CNC2",
          status: "running"
        },
        {
          name: "Macchina CNC3",
          status: "idle"
        },
        {
          name: "Macchina CNC4",
          status: "running"
        }
      ]
    }
  ]

// Heat‑map dati di esempio (0–100% utilisation)
const machines = [
  { id: "CNC‑01", hours: [40, 30, 60, 75, 55, 20, 10] },
  { id: "LATHE‑02", hours: [30, 50, 45, 80, 70, 60, 35] },
  { id: "ASMB‑01", hours: [20, 25, 40, 60, 50, 30, 15] },
  { id: "TEST‑01", hours: [50, 70, 80, 90, 85, 60, 40] }
];

function KPI({ icon: Icon, label, value, className = "" }) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4 }}
      className={`rounded-2xl bg-zinc-800 p-4 flex items-center space-x-4 ${className}`}
    >
      <Icon className="w-6 h-6 text-teal-400 shrink-0" />
      <div>
        <div className="text-sm text-zinc-400 uppercase">{label}</div>
        <div className="text-2xl font-bold text-zinc-100">{value}</div>
      </div>
    </motion.div>
  );
}

function ProductionChart() {
  return (
    <div className="rounded-2xl bg-zinc-800 p-4">
      <h2 className="text-zinc-100 text-lg mb-2">Production vs Target</h2>
      <ResponsiveContainer width="100%" height={200}>
        <AreaChart
          data={production}
          margin={{ top: 10, right: 20, left: 0, bottom: 0 }}
        >
          <defs>
            <linearGradient id="prodGradient" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#14b8a6" stopOpacity={0.8} />
              <stop offset="95%" stopColor="#14b8a6" stopOpacity={0} />
            </linearGradient>
          </defs>
          <CartesianGrid strokeDasharray="3 3" strokeOpacity={0.1} />
          <XAxis dataKey="time" stroke="#a1a1aa" />
          <YAxis stroke="#a1a1aa" />
          <ChartTooltip cursor={{ stroke: "#4b5563", strokeWidth: 1 }} />
          <Area
            type="monotone"
            dataKey="value"
            stroke="#14b8a6"
            fill="url(#prodGradient)"
            strokeWidth={2}
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}

function PlantMap() {
  return (
    <div className="rounded-2xl bg-zinc-800 overflow-hidden">
      <MapContainer
        center={[20, 0]}
        zoom={1}
        style={{ height: "256px", width: "100%" }}
        scrollWheelZoom={false}
        zoomControl={true}
      >
        <TileLayer
          attribution="&copy; <a href='https://www.openstreetmap.org/copyright'>OpenStreetMap</a> contributors"
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        {plants.map((p) => (
          <CircleMarker
            key={p.name}
            center={p.position}
            radius={12}
            pathOptions={{
              fill: true,
              color: "#14b8a6",
              weight: 3,
              fillColor: "#14b8a6",
              fillOpacity: 0.7
            }}
          >
            <MapTooltip>
              <div className="text-sm text-gray-900">
                <strong className="block mb-1">{p.name}</strong>
                <div>
                  Running: <span className="text-green-600">{p.running}</span>
                </div>
                <div>
                  Idle: <span className="text-yellow-600">{p.idle}</span>
                </div>
                <div>
                  Alarm: <span className="text-red-600">{p.alarm}</span>
                </div>
              </div>
            </MapTooltip>
          </CircleMarker>
        ))}
      </MapContainer>
    </div>
  );
}

function EventLogTable() {
  return (
    <div className="rounded-2xl bg-zinc-800 p-4 overflow-auto">
      <h2 className="text-zinc-100 text-lg mb-2">Event Log</h2>
      <table className="w-full text-left text-sm">
        <thead className="uppercase text-zinc-400 border-b border-zinc-700">
          <tr>
            <th className="py-2">Severity</th>
            <th className="py-2">Time</th>
            <th className="py-2">Plant</th>
            <th className="py-2">Description</th>
          </tr>
        </thead>
        <tbody>
          {events.map((e, i) => (
            <tr
              key={i}
              className="border-b border-zinc-700 hover:bg-zinc-700/30"
            >
              <td className="py-2 font-medium text-zinc-200">{e.severity}</td>
              <td className="py-2 text-zinc-300">{e.time}</td>
              <td className="py-2 text-zinc-300">{e.plant}</td>
              <td className="py-2 text-zinc-300">{e.message}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function MachineHeatMap() {
  return (
    <div className="rounded-2xl bg-zinc-800 p-4">
      <h2 className="text-zinc-100 text-lg mb-4">Machine Utilization</h2>
      <div className="space-y-2">
        {machines.map((m) => (
          <div key={m.id} className="flex items-center space-x-2">
            <span className="w-24 text-sm text-zinc-300">{m.id}</span>
            <div className="flex-grow grid grid-cols-7 gap-1">
              {m.hours.map((u, idx) => (
                <div
                  key={idx}
                  className="h-4 rounded-sm"
                  style={{ backgroundColor: `hsl(174, 60%, ${20 + u * 0.6}%)` }}
                  title={`${u}%`}
                />
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

/**
 * Component Status factory
 */

const StatusFactory = () => {
  return (
    <div className="bg-zinc-800 rounded-2xl p-4 flex flex-col">
      <div>
        <p className="text-white text-lg mb-2">Status Factory</p>
      </div>
      <div className="flex">
        {factory.map(f => {
          return (
            <div className="bg-zinc-900 p-2 m-1">
              <p className="text-2xl opacity-55">{f.name[0].toUpperCase()}</p>
              <div>
                {f.machines.map(m => {
                  return (
                  <div className={`w-10 h-10 m-1 rounded ${m.status == "error" ? "bg-red-500" : m.status == "running" ? "bg-green-400" : "bg-gray-500"}`}></div>)
                })}
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}

// ──────────────────────────────────────────────────────────────────────────────
//  MAIN DASHBOARD
// ──────────────────────────────────────────────────────────────────────────────
export default function Dashboard() {
  const [selectedPlant, setSelectedPlant] = useState("All Plants");

  return (
    <div className="min-h-screen w-screen bg-zinc-900 text-zinc-100 p-6 space-y-6 font-sans flex flex-row">

      <div className="flex flex-col w-full space-y-6">
        {/* Header */}
        <header className="flex items-center justify-between">
          <h1 className="text-2xl font-semibold tracking-tight">Mekspresso</h1>
          <select
            value={selectedPlant}
            onChange={(e) => setSelectedPlant(e.target.value)}
            className="bg-zinc-800 text-zinc-100 p-2 rounded-lg"
          >
            <option>All Plants</option>
            {plants.map((p) => (
              <option key={p.name}>{p.name}</option>
            ))}
          </select>
        </header>

        {/* KPI Row */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <KPI icon={Package} label="Output Today" value={kpi.output} />
          <KPI icon={TrendingUp} label="OEE" value={`${kpi.oee}%`} />
          <KPI icon={Layers} label="Active Lots" value={kpi.lots} />
          <KPI icon={Bell} label="Open Alarms" value={kpi.alarms} />
        </div>

        {/* Production Chart and status factory*/}
        <div className="grid md:grid-cols-2 gap-4">
          <ProductionChart />
          <StatusFactory />
        </div>

        {/* Map + Event Log */}
        <div className="grid md:grid-cols-2 gap-4">
          <PlantMap />
          <EventLogTable />
        </div>

        {/* Heat Map */}
        <MachineHeatMap />
      </div>
    </div>
  );
}
