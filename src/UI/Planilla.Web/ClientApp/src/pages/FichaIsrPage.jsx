import React, { useEffect, useMemo, useState } from 'react';
import { Loader2, FileSpreadsheet, Save, Info } from 'lucide-react';
import toast from 'react-hot-toast';
import { api } from '../services/api';
import { formatDate } from '../utils/date';

// ============================================================
// Ficha anual de ISR
//
// Reproduce el libro que el contador lleva a mano: una fila por corrida,
// con la proyección del año y el impuesto que se fue reteniendo. Cada
// columna existe para poder decirle a un empleado por qué le descontaron
// lo que le descontaron en una quincena concreta.
// ============================================================

const fmt = (n) => {
  const v = typeof n === 'number' ? n : parseFloat(n ?? 0);
  return isNaN(v) ? '0.00' : v.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
};

const fmtDate = (d) => (d ? formatDate(d) : '—');

const ANIO_ACTUAL = new Date().getFullYear();
const ANIOS = [ANIO_ACTUAL - 2, ANIO_ACTUAL - 1, ANIO_ACTUAL, ANIO_ACTUAL + 1];

const SALDOS_VACIOS = {
  ingresoGravableInicial: '',
  decimoInicial: '',
  isrRetenidoInicial: '',
};

export default function FichaIsrPage() {
  const [empleados, setEmpleados] = useState([]);
  const [empleadoId, setEmpleadoId] = useState('');
  const [anio, setAnio] = useState(ANIO_ACTUAL);

  const [ficha, setFicha] = useState(null);
  const [isLoadingEmpleados, setIsLoadingEmpleados] = useState(true);
  const [isLoadingFicha, setIsLoadingFicha] = useState(false);

  const [showSaldos, setShowSaldos] = useState(false);
  const [saldos, setSaldos] = useState(SALDOS_VACIOS);
  const [savingSaldos, setSavingSaldos] = useState(false);

  useEffect(() => { loadEmpleados(); }, []);

  useEffect(() => {
    if (empleadoId) loadFicha();
    else setFicha(null);
  }, [empleadoId, anio]);

  const loadEmpleados = async () => {
    try {
      setIsLoadingEmpleados(true);
      const data = await api.get('/api/empleados');
      const lista = Array.isArray(data) ? data : [];
      setEmpleados(lista);
      if (lista.length > 0) setEmpleadoId(String(lista[0].id));
    } catch (error) {
      toast.error(error.message || 'No se pudo cargar la lista de empleados');
    } finally {
      setIsLoadingEmpleados(false);
    }
  };

  const loadFicha = async () => {
    try {
      setIsLoadingFicha(true);
      const data = await api.get(`/api/acumulados-fiscales/${empleadoId}/ficha/${anio}`);
      setFicha(data);
    } catch (error) {
      setFicha(null);
      toast.error(error.message || 'No se pudo cargar la ficha');
    } finally {
      setIsLoadingFicha(false);
    }
  };

  const abrirSaldos = async () => {
    try {
      const data = await api.get(`/api/acumulados-fiscales/${empleadoId}/saldos/${anio}`);
      setSaldos({
        ingresoGravableInicial: String(data?.ingresoGravableInicial ?? 0),
        decimoInicial: String(data?.decimoInicial ?? 0),
        isrRetenidoInicial: String(data?.isrRetenidoInicial ?? 0),
      });
      setShowSaldos(true);
    } catch (error) {
      toast.error(error.message || 'No se pudieron cargar los saldos iniciales');
    }
  };

  const guardarSaldos = async () => {
    const valores = {
      ingresoGravableInicial: parseFloat(saldos.ingresoGravableInicial) || 0,
      decimoInicial: parseFloat(saldos.decimoInicial) || 0,
      isrRetenidoInicial: parseFloat(saldos.isrRetenidoInicial) || 0,
    };

    if (Object.values(valores).some(v => v < 0)) {
      toast.error('Los saldos iniciales no pueden ser negativos');
      return;
    }

    try {
      setSavingSaldos(true);
      await api.put(`/api/acumulados-fiscales/${empleadoId}/saldos/${anio}`, {
        empleadoId: Number(empleadoId),
        anio,
        ...valores,
      });
      toast.success('Saldos iniciales guardados');
      setShowSaldos(false);
      await loadFicha();
    } catch (error) {
      toast.error(error.message || 'No se pudieron guardar los saldos');
    } finally {
      setSavingSaldos(false);
    }
  };

  // El año recién empezado todavía no tiene un impuesto real contra el que
  // comparar: la diferencia solo dice algo con las corridas del año completas.
  const anioCompleto = useMemo(() => {
    if (!ficha) return false;
    const regulares = ficha.filas.filter(f => !f.esDecimo).length;
    return regulares >= Math.floor(ficha.periodosEquivalentes * 12 / 13);
  }, [ficha]);

  if (isLoadingEmpleados) return (
    <div className="flex items-center justify-center h-64">
      <Loader2 className="w-8 h-8 animate-spin text-blue-500" />
    </div>
  );

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-start justify-between gap-4 flex-wrap">
        <div>
          <h1 className="text-3xl font-bold text-white">Ficha Anual de ISR</h1>
          <p className="text-gray-400 mt-1">
            Método acumulativo — cada corrida cobra la diferencia entre el impuesto debido y el ya retenido
          </p>
        </div>
        {empleadoId && (
          <button
            onClick={abrirSaldos}
            className="flex items-center gap-2 px-4 py-2 bg-slate-700 hover:bg-slate-600 text-white rounded-lg font-medium transition-colors"
          >
            <FileSpreadsheet className="w-4 h-4" /> Saldos iniciales
          </button>
        )}
      </div>

      {/* Filtros */}
      <div className="flex items-center gap-4 flex-wrap">
        <div className="flex items-center gap-2">
          <label htmlFor="ficha-empleado" className="text-sm text-gray-400">Empleado:</label>
          <select
            id="ficha-empleado"
            value={empleadoId}
            onChange={e => setEmpleadoId(e.target.value)}
            className="bg-slate-700 border border-slate-600 text-white rounded-lg px-3 py-1.5 text-sm min-w-64"
          >
            {empleados.length === 0 && <option value="">No hay empleados</option>}
            {empleados.map(e => (
              <option key={e.id} value={e.id}>{e.nombre} {e.apellido}</option>
            ))}
          </select>
        </div>
        <div className="flex items-center gap-2">
          <label htmlFor="ficha-anio" className="text-sm text-gray-400">Año:</label>
          <select
            id="ficha-anio"
            value={anio}
            onChange={e => setAnio(Number(e.target.value))}
            className="bg-slate-700 border border-slate-600 text-white rounded-lg px-3 py-1.5 text-sm"
          >
            {ANIOS.map(y => <option key={y} value={y}>{y}</option>)}
          </select>
        </div>
      </div>

      {isLoadingFicha && (
        <div className="flex items-center justify-center h-40">
          <Loader2 className="w-8 h-8 animate-spin text-blue-500" />
        </div>
      )}

      {!isLoadingFicha && ficha && (
        <>
          {/* Resumen */}
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <Tarjeta titulo="Ingreso gravable del año" valor={fmt(ficha.totalGravable)} />
            <Tarjeta titulo="Décimo del año" valor={fmt(ficha.totalDecimo)} />
            <Tarjeta titulo="ISR retenido" valor={fmt(ficha.totalIsrRetenido)} acento="text-blue-400" />
            <Tarjeta
              titulo={anioCompleto ? 'ISR del año / diferencia' : 'ISR del año (parcial)'}
              valor={fmt(ficha.isrDelAnioSegunIngresoReal)}
              nota={anioCompleto
                ? (ficha.diferenciaRetenido >= 0
                    ? `Retenido de más: ${fmt(ficha.diferenciaRetenido)}`
                    : `Falta retener: ${fmt(Math.abs(ficha.diferenciaRetenido))}`)
                : 'El año todavía no está completo'}
              acento={anioCompleto && Math.abs(ficha.diferenciaRetenido) > 0.01
                ? 'text-amber-400'
                : 'text-emerald-400'}
            />
          </div>

          <div className="text-sm text-gray-400 flex items-start gap-2">
            <Info className="w-4 h-4 mt-0.5 shrink-0" />
            <p>
              Frecuencia <span className="text-gray-200">{ficha.frecuencia}</span> —
              el año se reparte en <span className="text-gray-200">{fmt(ficha.periodosEquivalentes)}</span> períodos
              equivalentes, porque el décimo tercer mes también tributa y entra en el reparto.
            </p>
          </div>

          {/* Libro por corrida */}
          <div className="bg-slate-800 border border-slate-700 rounded-xl overflow-x-auto">
            <table className="w-full text-sm min-w-[1100px]">
              <thead>
                <tr className="bg-slate-700/50 text-gray-400 text-left">
                  <th className="px-3 py-3 font-medium">#</th>
                  <th className="px-3 py-3 font-medium">Corrida</th>
                  <th className="px-3 py-3 font-medium">Pago</th>
                  <th className="px-3 py-3 font-medium text-right">Bruto</th>
                  <th className="px-3 py-3 font-medium text-right">Seg. Social</th>
                  <th className="px-3 py-3 font-medium text-right">Gravable</th>
                  <th className="px-3 py-3 font-medium text-right">Acumulado</th>
                  <th className="px-3 py-3 font-medium text-right">Períodos</th>
                  <th className="px-3 py-3 font-medium text-right">Proyectado</th>
                  <th className="px-3 py-3 font-medium text-right">ISR anual</th>
                  <th className="px-3 py-3 font-medium text-right">Debido</th>
                  <th className="px-3 py-3 font-medium text-right">Descontado</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-700">
                {ficha.filas.length === 0 && (
                  <tr>
                    <td colSpan={12} className="px-4 py-10 text-center text-gray-400">
                      Este empleado no tiene corridas en {anio}
                    </td>
                  </tr>
                )}
                {ficha.filas.map((f, i) => (
                  <tr
                    key={`${f.concepto}-${i}`}
                    className={`text-gray-200 hover:bg-slate-700/30 transition-colors ${
                      f.esDecimo ? 'bg-slate-700/20' : ''
                    }`}
                  >
                    <td className="px-3 py-2 text-gray-400">{f.periodo}</td>
                    <td className="px-3 py-2">
                      {f.concepto}
                      {f.esDecimo && (
                        <span className="ml-2 px-1.5 py-0.5 rounded text-[10px] bg-purple-900 text-purple-200">
                          décimo
                        </span>
                      )}
                    </td>
                    <td className="px-3 py-2 text-gray-400">{fmtDate(f.fechaPago)}</td>
                    <td className="px-3 py-2 text-right font-mono">{fmt(f.bruto)}</td>
                    <td className="px-3 py-2 text-right font-mono text-gray-400">{fmt(f.seguroSocial)}</td>
                    <td className="px-3 py-2 text-right font-mono">{fmt(f.gravable)}</td>
                    <td className="px-3 py-2 text-right font-mono text-gray-400">{fmt(f.gravableAcumulado)}</td>
                    <td className="px-3 py-2 text-right font-mono text-gray-400">
                      {Number(f.periodoEquivalente).toFixed(3)}
                    </td>
                    <td className="px-3 py-2 text-right font-mono text-gray-400">{fmt(f.ingresoAnualProyectado)}</td>
                    <td className="px-3 py-2 text-right font-mono text-gray-400">{fmt(f.isrAnualProyectado)}</td>
                    <td className="px-3 py-2 text-right font-mono text-gray-400">{fmt(f.isrDebidoAcumulado)}</td>
                    <td className="px-3 py-2 text-right font-mono text-blue-400">{fmt(f.isrRetenido)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}

      {/* Modal de saldos iniciales */}
      {showSaldos && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4">
          <div className="bg-slate-800 border border-slate-700 rounded-xl w-full max-w-lg p-6 space-y-4">
            <div>
              <h2 className="text-xl font-bold text-white">Saldos iniciales {anio}</h2>
              <p className="text-sm text-gray-400 mt-1">
                Lo que el empleado ya traía acumulado cuando la empresa entró a Pagly.
                Sin esto, migrar a mitad de año le vuelve a cobrar un impuesto que ya pagó.
              </p>
            </div>

            <CampoSaldo
              id="saldo-gravable"
              label="Ingreso gravable acumulado"
              ayuda="Bruto menos Seguro Social, sumado de enero a la migración"
              value={saldos.ingresoGravableInicial}
              onChange={v => setSaldos(s => ({ ...s, ingresoGravableInicial: v }))}
            />
            <CampoSaldo
              id="saldo-decimo"
              label="Décimo pagado en el año"
              ayuda="Partidas de décimo ya pagadas antes de migrar"
              value={saldos.decimoInicial}
              onChange={v => setSaldos(s => ({ ...s, decimoInicial: v }))}
            />
            <CampoSaldo
              id="saldo-isr"
              label="ISR ya retenido"
              ayuda="Se descuenta del impuesto debido para no cobrarlo dos veces"
              value={saldos.isrRetenidoInicial}
              onChange={v => setSaldos(s => ({ ...s, isrRetenidoInicial: v }))}
            />

            <div className="flex justify-end gap-3 pt-2">
              <button
                onClick={() => setShowSaldos(false)}
                className="px-4 py-2 text-gray-300 hover:text-white transition-colors"
              >
                Cancelar
              </button>
              <button
                onClick={guardarSaldos}
                disabled={savingSaldos}
                className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:opacity-60 text-white rounded-lg font-medium transition-colors"
              >
                {savingSaldos ? <Loader2 className="w-4 h-4 animate-spin" /> : <Save className="w-4 h-4" />}
                Guardar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function Tarjeta({ titulo, valor, nota, acento = 'text-white' }) {
  return (
    <div className="bg-slate-800 border border-slate-700 rounded-xl p-4">
      <p className="text-sm text-gray-400">{titulo}</p>
      <p className={`text-2xl font-bold font-mono mt-1 ${acento}`}>B/. {valor}</p>
      {nota && <p className="text-xs text-gray-400 mt-1">{nota}</p>}
    </div>
  );
}

function CampoSaldo({ id, label, ayuda, value, onChange }) {
  return (
    <div>
      <label htmlFor={id} className="block text-sm text-gray-300 mb-1">{label}</label>
      <input
        id={id}
        type="number"
        step="0.01"
        min="0"
        value={value}
        onChange={e => onChange(e.target.value)}
        className="w-full bg-slate-700 border border-slate-600 text-white rounded-lg px-3 py-2 font-mono"
      />
      <p className="text-xs text-gray-400 mt-1">{ayuda}</p>
    </div>
  );
}
